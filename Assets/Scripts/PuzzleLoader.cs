using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLoader : MonoBehaviour {
    public void loadPuzzle(string a) {
        Utils.loadPuzzle("PuzzleStage" + a);
    }
}
