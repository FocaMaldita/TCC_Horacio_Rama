using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropManager : MonoBehaviour {

    public static GameObject currentDragItem = null;
    public InstructionList catList, dogList;

    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            onFinishDrag();
        }
        if (Input.touchCount >= 1 && Input.touches[0].phase == TouchPhase.Ended) {
            onFinishDrag();
        }
    }

    public void onFinishDrag() {
        if (!Interpreter.isInterpreting) {
            if (currentDragItem) {
                if (catList) {
                    var catIndex = Utils.ListContainsNull(catList.list);
                    if (catIndex >= 0) {
                        catList.insertAtPosition(catIndex);
                    }
                }
                if (dogList) {
                    var dogIndex = Utils.ListContainsNull(dogList.list);
                    if (dogIndex >= 0) {
                        dogList.insertAtPosition(dogIndex);
                    }
                }
                Destroy(currentDragItem);
                currentDragItem = null;
            }
        }
    }

}
