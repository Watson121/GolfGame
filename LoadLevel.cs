using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


/// <summary>
/// Loading the level objects into the game with the use of a file.
/// </summary>
public class LoadLevel : MonoBehaviour
{

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
    private GameObject levelObjType;

    // Start is called before the first frame update
    void Start()
    {
        LoadingLevel();
    }

    /// <summary>
    /// Loading Level by getting file
    /// </summary>
    public void LoadingLevel()
    {
        if (File.Exists(Application.dataPath + "/EditorSaves/" + "test.savelevel"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/EditorSaves/" + "test.savelevel", FileMode.Open);
            SaveLevel level = (SaveLevel)bf.Deserialize(file);
            file.Close();

            for(int i = 0; i < level.numberOfLevelObjects; i++)
            {
                GameObject levelPiece;
                Vector3 position = new Vector3(level.levelObjectPositions[i][0], level.levelObjectPositions[i][1], level.levelObjectPositions[i][2]);

                switch (level.levelObjectType[i])
                {
                    case 0:
                        levelObjType = startingLevelPiece;
                        break;
                    case 1:
                        levelObjType = endingLevelPiece;
                        break;
                    case 2:
                        levelObjType = normalLevelPiece;
                        break;
                    case 3:
                        levelObjType = cornerLevelPiece;
                        break;
                    case 4:
                        levelObjType = sideLevelPiece;
                        break;
                    case 5:
                        levelObjType = waterPiece;
                        break;
                    case 6:
                        levelObjType = checkpointPiece;
                        break;
                    case 7:
                        levelObjType = slantedCornerPiece;
                        break;
                    case 8:
                        levelObjType = rampUpPiece;
                        break;
                    case 9:
                        levelObjType = rampUpBothBarrierPiece;
                        break;
                    case 10:
                        levelObjType = rampUpRightBarrierPiece;
                        break;
                    case 11:
                        levelObjType = rampUpLeftBarrierPiece;
                        break;
                }

                levelPiece = Instantiate(levelObjType, position, Quaternion.identity);

            }
        }
    }
}
