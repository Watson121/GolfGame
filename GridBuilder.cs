using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main Class for building the grid in the level editor
/// </summary>
public class GridBuilder : MonoBehaviour
{

    #region VARIABLES

    /// <summary>
    /// Grid Square - Prefab of the Grid Square that will spawn
    /// Grid Width - The Width of the Grid
    /// Grid Height - The Height of the Grid
    /// </summary>
    [Header("Grid Setup")]
    public GameObject gridSquare;
    public uint gridWidth;
    public uint gridHeight;

    /// <summary>
    /// List of the grid sqaures
    /// </summary>
    [Header("Grid Squares")]
    public List<GridSquare> gridSquares;

    [Header("Golf Ball & Camera")]
    public GameObject golfBall;
    public GameObject gameCamera;
    private CameraControlsEditor cameraControlsEditor;

    //The Different User Interfaces that the player will interact with
    private GameObject playerUI;
    private GameObject editorUI;

    /// <summary> User Interface
    /// No Spawn Point - If there is no Spawn Point in the level, this message will appear.
    /// No End Point - If there is no End Point in the level, this message will appear.
    /// Level Input Window - Input window where the player enters file name for the level
    /// Level Name Input - Text space where the player enters the file name
    /// </summary>
    private GameObject noSpawnPointErr;
    private GameObject noEndPointErr;
    private GameObject levelInputWindow;
    private TMP_InputField levelNameInput;
    private GameObject pauseMenu;

    //Starting point for spwaning the grid
    private Vector3 spawnPos = new Vector3(0, 0, 0);
    private float xPos = 2;
    private float yPos = 0;
    private float zPos = 2;

    //Is the level being play tested or not
    [Header("LEVEL TESTING")]
    public bool levelTesting;

    //List of objects that are placed around the editor level. This is what will be saved by the game.
    [Header("LEVEL OBJECTS")]
    public List<GameObject> levelObjects = new List<GameObject>();

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        EditorSetup();
        CreateGrid();
    }

    private void Update()
    {
        //Opening the Pause Menu
        if (Input.GetButtonDown("Pause"))
        {
            pauseMenu.SetActive(true);
        }
    }

    //Setting up the Editor
    void EditorSetup()
    {
        //Finding In Game Camera, and Controls
        gameCamera = GameObject.Find("Level Editor Camera");
        cameraControlsEditor = gameCamera.GetComponent<CameraControlsEditor>();

        //Finding Level Name Input
        levelNameInput = GameObject.Find("InputLevelName").GetComponent<TMP_InputField>();

        //Finding the error messages
        noSpawnPointErr = GameObject.Find("NoSpawnError");
        noEndPointErr = GameObject.Find("NoEndPointError");
        levelInputWindow = GameObject.Find("LevelNameWindow");
        noSpawnPointErr.SetActive(false);
        noEndPointErr.SetActive(false);
        levelInputWindow.SetActive(false);

        //Finding the different User interfaces
        playerUI = GameObject.Find("Player UI (Editor)");
        editorUI = GameObject.Find("Editor User Interface");
        playerUI.SetActive(false);
        editorUI.SetActive(true);

        levelTesting = false;

        GameObject.Find("GridLevel").GetComponent<TMP_InputField>().text = yPos.ToString();

    }

    /// <summary>
    /// Creating the Grid. Grid Size is controlled by width and height
    /// </summary>
    public void CreateGrid()
    {
        spawnPos = new Vector3(0, 0, 0);

        for (int i = 0; i < gridSquares.Count; i++)
        {
            Destroy(gridSquares[i].gameObject);
        }

        gridSquares.Clear();

        for (int i = 0; i <= gridWidth; i++)
        {
            for (int k = 0; k <= gridHeight; k++)
            {
                GameObject square = Instantiate(gridSquare, new Vector3(i * xPos, yPos, k * zPos), Quaternion.identity, transform);
                square.name = "Grid_Square_" + i + "_" + k;
                gridSquares.Add(square.GetComponent<GridSquare>());
            }
        }
    }

    //Clearing grid of all elements
    public void ClearGrid()
    {
        levelObjects.Clear();

        for(int i = 0; i < gridSquares.Count; i++)
        {
            gridSquares[i].ClearGridSquare();
        }
    }

    //Setting the Grid Width uint Value
    public void SetGridWidth(string width)
    {
        gridWidth = uint.Parse(width);
    }

    //Setting the Grid Height uint Value
    public void SetGridHeight(string height)
    {
        gridHeight = uint.Parse(height);
    }

    /// <summary>
    /// Setting Vertical Position of the Grid
    /// </summary>
    /// <param name="move"></param>
    public void SettingGridVerticalPos(float move)
    {
        yPos += move;
        yPos = Mathf.Clamp(yPos, -5, 5);

        GameObject.Find("GridLevel").GetComponent<TMP_InputField>().text = yPos.ToString();
        CreateGrid();
    }


    #region LEVEL TESTING - Functions related to level testing

    //Play testing the game level before you create the level file
    public void TestLevel()
    {

        GameObject startingPosition = null;
        startingPosition = GameObject.FindGameObjectWithTag("StartingPoint");

        GameObject endPoisiton = null;
        endPoisiton = GameObject.FindGameObjectWithTag("EndingPoint");


        if(!startingPosition)
        {
            noSpawnPointErr.SetActive(true);
            return;
        }

        if (!endPoisiton)
        {
            noEndPointErr.SetActive(true);
            return;
        }

        editorUI.SetActive(false);
        playerUI.SetActive(true);

        Destroy(GameObject.FindGameObjectWithTag("GolfBall"));

        GameObject ball = Instantiate(golfBall, startingPosition.transform.GetChild(0).position, Quaternion.identity);
        cameraControlsEditor.enabled = false;
        levelTesting = true;
  
    }

    //Stop Playtesting the game level 
    public void StopTesting()
    {
        editorUI.SetActive(true);
        playerUI.SetActive(false);

        GameObject ball = GameObject.FindGameObjectWithTag("GolfBall");

        Destroy(ball);

        cameraControlsEditor.enabled = true;

        levelTesting = false;
    }

    #endregion

    #region SAVING LEVEL - Functions related to saving the level. 

    /// <summary>
    /// Saving the Level, so that it can be played later in the main game.
    /// </summary>
    public void SavingLevel()
    {

        GameObject startingPosition = null;
        startingPosition = GameObject.FindGameObjectWithTag("StartingPoint");

        GameObject endPoisiton = null;
        endPoisiton = GameObject.FindGameObjectWithTag("EndingPoint");

        if (!startingPosition)
        {
            noSpawnPointErr.SetActive(true);
            return;
        }

        if (!endPoisiton)
        {
            noEndPointErr.SetActive(true);
            return;
        }

        SaveLevel level = CreateLevelFile();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.dataPath + "/EditorSaves/" + levelNameInput.text + ".savelevel");
        bf.Serialize(file, level);
        file.Close();

        Debug.Log(Application.persistentDataPath);
        Debug.Log("Level has been saved!");

    }

    /// <summary>
    /// Creating a Level File. This file will contain all of the information that will be used for creating the level in the main game scene.
    /// </summary>
    /// <returns>A Save Level Object</returns>
    private SaveLevel CreateLevelFile()
    {
        SaveLevel level = new SaveLevel();

        foreach (GameObject obj in levelObjects)
        {
            level.levelObjectPositions.Add(new float[3] { obj.transform.position.x, obj.transform.position.y, obj.transform.position.z });

            switch (obj.tag)
            {
                case "StartingPoint":
                    level.levelObjectType.Add(0);
                    break;
                case "EndingPoint":
                    level.levelObjectType.Add(1);
                    break;
                case "NormalPiece":
                    level.levelObjectType.Add(2);
                    break;
                case "CornerPiece":
                    level.levelObjectType.Add(3);
                    break;
                case "SidePiece":
                    level.levelObjectType.Add(4);
                    break;
                case "WaterPiece":
                    level.levelObjectType.Add(5);
                    break;
                case "Checkpoint":
                    level.levelObjectType.Add(6);
                    break;
                case "SlantedCorner":
                    level.levelObjectType.Add(7);
                    break;
                case "RampUp":
                    level.levelObjectType.Add(8);
                    break;
                case "RampBarrierBoth":
                    level.levelObjectType.Add(9);
                    break;
                case "RampBarrierRight":
                    level.levelObjectType.Add(10);
                    break;
                case "RampBarrierLeft":
                    level.levelObjectType.Add(11);
                    break;
           
            }



            //if (obj.tag == "StartingPoint") level.levelObjectType.Add(0);
            //else if (obj.tag == "EndingPoint") level.levelObjectType.Add(1);
            //else if (obj.tag == "NormalPiece") level.levelObjectType.Add(2);
            //else if (obj.tag == "CornerPiece") level.levelObjectType.Add(3);
            //else if (obj.tag == "SidePiece") level.levelObjectType.Add(4);
            //else if (obj.tag == "WaterPiece") level.levelObjectType.Add(5);
            //else if (obj.tag == "Checkpoint") level.levelObjectType.Add(6);
            //else if (obj.tag == "SlantedCorner") level.levelObjectType.Add(7);
            //else Debug.LogWarning("Object Type Not Found! FIX! " + obj.name);
            
            level.numberOfLevelObjects++;
        }

        return level;

    }

    #endregion

}
