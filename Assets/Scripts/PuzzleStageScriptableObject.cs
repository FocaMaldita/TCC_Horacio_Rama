using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleStage", menuName = "ScriptableObjects/PuzzleStageScriptableObject", order = 1)]
public class PuzzleStageScriptableObject : ScriptableObject {
    [HideInInspector]
    public int rowCount, colCount;

    public bool hasCat = true, hasDog = true;
    public bool hasMove = true, hasGrab = true, hasWait = true;

    public int catPositionX, catPositionY;
    public int dogPositionX, dogPositionY;

    [System.Serializable]
    public class Row {
        public PuzzleManager.PuzzleObject[] entries;
    }
    
    [HideInInspector]
    public Row[] matrix;
}

public class PuzzleStage {
    public int rowCount, colCount;
    
    public PuzzleStageScriptableObject.Row[] matrix;
}
