using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveLevel
{

    public List<float[]> levelObjectPositions = new List<float[]>();
    public List<int> levelObjectType = new List<int>();
    public int numberOfLevelObjects = 0;

}
