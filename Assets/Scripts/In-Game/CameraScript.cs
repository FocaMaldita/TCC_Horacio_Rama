using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraScript : MonoBehaviour {
    [SerializeField]
    private PuzzleManager puzzle;
    [SerializeField]
    private int pixelsPerUnit;
    [SerializeField]
    private float offsetY;

    void Start() {
        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = puzzle.kindMatrix.GetLength(0) / puzzle.kindMatrix.GetLength(1);

        if (screenRatio >= targetRatio) {
            Camera.main.orthographicSize = puzzle.kindMatrix.GetLength(1) + offsetY / 2;
        } else {
            float differenceInSize = targetRatio / screenRatio;
            Debug.Log("differenceInSize: " + differenceInSize);
            Camera.main.orthographicSize = (puzzle.kindMatrix.GetLength(1) + offsetY / 2) * differenceInSize;
        }
    }
}
