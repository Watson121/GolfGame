using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


/// <summary>
/// Main Script for the interaction with the Grid Squares in the Level Editor.
/// Will also spawn the level piece
/// </summary>
public class GridSquare : MonoBehaviour
{

    #region VARIABLES

    //Is the Player hovering of this grid square or not
    [SerializeField] private bool selected;

    [Header("Grid Manager")]
    public GridBuilder grid;

    [Header("Editor Interface")]
    public TMP_Dropdown levelPieceSelector;

    //Grid Square Mesh Renderer. This will disable when a piece of the level is placed down.
    private MeshRenderer gridSquare;

    /// <summary>
    /// List of objects that the player can place in the level
    /// </summary>
    [Header("The Type of Level Pieces that can be placed")]
    public GameObject levelPieceToPlace;
    public GameObject normalLevelPiece;
    public GameObject cornerLevelPiece;
    public GameObject sideLevelPiece;
    public GameObject startingLevelPiece;
    public GameObject endingLevelPiece;
    public GameObject checkpointPiece;
    public GameObject waterPiece;
    public GameObject slantedCornerPiece;
    public GameObject rampUpPiece;
    public GameObject rampUpRightBarrierPiece;
    public GameObject rampUpLeftBarrierPiece;
    public GameObject rampUpBothBarrierPiece;
    public GameObject wallPiece;
    [SerializeField] GameObject levelPiece;

    /// <summary>
    /// When the player hovers over a grid square, the current level piece they want to place will highlight.
    /// </summary>
    private GameObject highlightObject;
    private GameObject normalPieceHighlight;
    private GameObject sidePieceHighlight;
    private GameObject cornerPieceHighlight;
    private GameObject startingLevelHighlight;
    private GameObject endinglevelHighlight;
    private GameObject checkpointHighlight;
    private GameObject waterHighlight;
    private GameObject slantedCornerHighlight;
    private GameObject rampUpHighlight;
    private GameObject rampUpLeftBarrierHighlight;
    private GameObject rampUpRightBarrierHighlight;
    private GameObject rampUpBothBarrierHighlight;
    private GameObject wallHighlight;

    //Rotation of the piece of level that will be placed
    private float xRotation = 0.0f;
    private float yRotation = 0.0f;
    

    #endregion

    // Start is called before the first frame update
    void Start()
    {

        GridSquareSetup();
        GettingEditorInterface();
        SetUpHighlights();
        
    }

    //Setting up the Grid Square
    void GridSquareSetup()
    {
        grid = GameObject.Find("Grid").GetComponent<GridBuilder>();
    }

    //Getting elements form the Editor User Interface
    void GettingEditorInterface()
    {
        levelPieceSelector = GameObject.Find("GolfCoursePieceDropdown").GetComponent<TMP_Dropdown>();
    }

    void SetUpHighlights()
    {
        gridSquare = GetComponent<MeshRenderer>();

        normalPieceHighlight = transform.GetChild(0).gameObject;
        cornerPieceHighlight = transform.GetChild(1).gameObject;
        sidePieceHighlight = transform.GetChild(2).gameObject;
        startingLevelHighlight = transform.GetChild(3).gameObject;
        endinglevelHighlight = transform.GetChild(4).gameObject;
        checkpointHighlight = transform.GetChild(5).gameObject;
        waterHighlight = transform.GetChild(6).gameObject;
        slantedCornerHighlight = transform.GetChild(7).gameObject;
        rampUpHighlight = transform.GetChild(8).gameObject;
        rampUpLeftBarrierHighlight = transform.GetChild(9).gameObject;
        rampUpRightBarrierHighlight = transform.GetChild(10).gameObject;
        rampUpBothBarrierHighlight = transform.GetChild(11).gameObject;
        wallHighlight = transform.GetChild(12).gameObject;
        highlightObject = normalPieceHighlight;

        normalPieceHighlight.SetActive(false);
        cornerPieceHighlight.SetActive(false);
        sidePieceHighlight.SetActive(false);
        startingLevelHighlight.SetActive(false);
        endinglevelHighlight.SetActive(false);
        checkpointHighlight.SetActive(false);
        waterHighlight.SetActive(false);
        slantedCornerHighlight.SetActive(false);
        rampUpHighlight.SetActive(false);
        rampUpLeftBarrierHighlight.SetActive(false);
        rampUpRightBarrierHighlight.SetActive(false);
        rampUpBothBarrierHighlight.SetActive(false);
        wallHighlight.SetActive(false);
        highlightObject.SetActive(false);
        
    }


    // Update is called once per frame
    void Update()
    {
        if (grid.levelTesting == false)
        {

            gridSquare.enabled = !grid.levelObjects.Find(x => levelPiece);
            
            if (selected == true && !IsMouseOverObjectWithIgnores())
            {
                DetermineLevelPiece();

                highlightObject.SetActive(true);
                highlightObject.transform.rotation = Quaternion.Euler(new Vector3(xRotation, yRotation, 0));

                SettingObjectRotation(highlightObject, -0.4f);

                //Left Click
                //Placing Level Piece Down
                if (Input.GetMouseButtonDown(0))
                {
                    if (levelPiece)
                    {
                        grid.levelObjects.Remove(levelPiece);
                        Destroy(levelPiece);
                    }

                    ///Checking to see if a starting Level Piece is already placed somewhere in the world.
                    ///If yes then destory the orignal and place the new one.
                    if(levelPieceToPlace == startingLevelPiece)
                    {
                        GameObject start = null;
                        start = GameObject.FindGameObjectWithTag("StartingPoint");

                        if (start)
                        {
                            grid.levelObjects.Remove(start);
                            Destroy(start);
                        }
                    }

                    /// Checking to see if a ending Level Piece is already placed somewhere in the world.
                    /// If yes then destroy the orignal and place the new one.
                    if (levelPieceToPlace == endingLevelPiece)
                    {
                        GameObject end = null;
                        end = GameObject.FindGameObjectWithTag("EndingPoint");

                        if (end)
                        {
                            grid.levelObjects.Remove(end);
                            Destroy(end);
                        }
                    }

                    levelPiece = Instantiate(levelPieceToPlace, transform.position, Quaternion.Euler(new Vector3(xRotation, yRotation, 0)));
                    
                    SettingObjectRotation(levelPiece, -0.5f);

                    grid.levelObjects.Add(levelPiece);

                }

                //Middle Mouse
                //Destroying Level Piece that is currently placed down
                if (Input.GetMouseButtonDown(2))
                {
                    grid.levelObjects.Remove(levelPiece);
                    if (levelPiece) Destroy(levelPiece);
                    
                }
            }
            else if (selected == false)
            {
                highlightObject.SetActive(false);
            }


            GridSquareControls();
            

        }
    }

    //Controlling the rotation of the level piece
    void GridSquareControls()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            yRotation += 90;
            if (yRotation > 360) yRotation = 0;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            yRotation -= 90;
            if (yRotation < 0) yRotation = 360;
        }
    }


    //Setting rotation of the object
    void SettingObjectRotation(GameObject obj, float yHeight)
    {
        if (yRotation == 0 || yRotation == 360)
        {
            obj.transform.position = transform.position + new Vector3(-1f, yHeight, 1.0f);
        }
        else if (yRotation == 90)
        {
            obj.transform.position = transform.position + new Vector3(1f, yHeight, 1f);
        }
        else if (yRotation == 180)
        {
            obj.transform.position = transform.position + new Vector3(1f, yHeight, -1f);
        }else if(yRotation == 270)
        {
            obj.transform.position = transform.position + new Vector3(-1f, yHeight, -1f);
        }
    }

    /// <summary>
    /// Determining the Level Piece that will be placed down by the player
    /// </summary>
    void DetermineLevelPiece()
    {
        switch (levelPieceSelector.value)
        {
            case 0:
                levelPieceToPlace = normalLevelPiece;
                highlightObject = normalPieceHighlight;
                break;
            case 1:
                levelPieceToPlace = cornerLevelPiece;
                highlightObject = cornerPieceHighlight;
                break;
            case 2:
                levelPieceToPlace = sideLevelPiece;
                highlightObject = sidePieceHighlight;
                break;
            case 3:
                levelPieceToPlace = startingLevelPiece;
                highlightObject = startingLevelHighlight;
                break;
            case 4:
                levelPieceToPlace = endingLevelPiece;
                highlightObject = endinglevelHighlight;
                break;
            case 5:
                levelPieceToPlace = checkpointPiece;
                highlightObject = checkpointHighlight;
                break;
            case 6:
                levelPieceToPlace = waterPiece;
                highlightObject = waterHighlight;
                break;
            case 7:
                levelPieceToPlace = slantedCornerPiece;
                highlightObject = slantedCornerHighlight;
                break;
            case 8:
                levelPieceToPlace = rampUpPiece;
                highlightObject = rampUpHighlight;
                break;
            case 9:
                levelPieceToPlace = rampUpBothBarrierPiece;
                highlightObject = rampUpBothBarrierHighlight;
                break;
            case 10:
                levelPieceToPlace = rampUpRightBarrierPiece;
                highlightObject = rampUpRightBarrierHighlight;
                break;
            case 11:
                levelPieceToPlace = rampUpLeftBarrierPiece;
                highlightObject = rampUpLeftBarrierHighlight;
                break;
            case 12:
                levelPieceToPlace = wallPiece;
                highlightObject = wallHighlight;
                break;
        }
    }

    public void ClearGridSquare()
    {
        if (levelPiece) Destroy(levelPiece);
        gridSquare.enabled = true;
    }

    #region Mouse Functionality

    private void OnMouseOver()
    {
        selected = true;
    }

    private void OnMouseExit()
    {
        selected = false;
    }

    //Checking if mouse is over UI or not
    private bool IsMouseOverObjectWithIgnores()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

        for(int i = 0; i < raycastResultList.Count; i++)
        {
            if(raycastResultList[i].gameObject.GetComponent<CanClickThrough>() != null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultList.Count > 0;
    }

    #endregion

    private void OnTriggerStay(Collider other)
    {
        levelPiece = other.gameObject;
    }


}
