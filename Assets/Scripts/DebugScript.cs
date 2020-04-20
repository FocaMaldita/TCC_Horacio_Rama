using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour {
    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Utils.loadPuzzle("PuzzleStage1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Utils.loadPuzzle("PuzzleStage2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Utils.loadPuzzle("PuzzleStage3");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            Utils.loadPuzzle("PuzzleStage4");
        }
    }

    public void loadPuzzle(string a) {
        Utils.loadPuzzle("PuzzleStage" + a);
    }
}
