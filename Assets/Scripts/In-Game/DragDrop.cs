using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class DragDrop : MonoBehaviour {

    public static GameObject currentDragItem = null;

    [SerializeField]
    private GameObject dragItem;

    public void onStartDrag() {
        currentDragItem = Instantiate(dragItem, transform);
        Debug.Log("aaaa");
    }

    public void onFinishDrag() {
        if (currentDragItem) {
            Destroy(currentDragItem);
            currentDragItem = null;
        }
    }
}
