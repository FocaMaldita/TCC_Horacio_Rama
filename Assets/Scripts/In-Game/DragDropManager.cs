using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour {

    public static GameObject currentDragItem = null;

    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            onFinishDrag();
        }
    }

    public void onFinishDrag() {
        if (!Interpreter.isInterpreting) {
            if (currentDragItem) {
                Destroy(currentDragItem);
                currentDragItem = null;
            }
        }
    }

}
