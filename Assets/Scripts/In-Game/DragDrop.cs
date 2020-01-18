using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DragDrop : MonoBehaviour {

    public static GameObject currentDragItem = null;

    public Utils.InstructionType instruction;

    [SerializeField]
    private GameObject dragItem;

    [SerializeField]
    private string dragItemIcon;

    private Color color;

    public void Start() {
        color = GetComponent<Image>().color;
        color.a = .5f;
    }

    public void onStartDrag() {
        Canvas canvas = FindObjectOfType<Canvas>();
        currentDragItem = Instantiate(dragItem,
                                      transform.position,
                                      Quaternion.identity,
                                      canvas.transform
                                      );
        currentDragItem.GetComponent<Image>().color = color;
        currentDragItem.GetComponentInChildren<Text>().text = dragItemIcon;
    }

    public void onFinishDrag() {
        if (currentDragItem) {
            Destroy(currentDragItem);
            currentDragItem = null;
        }
    }
}
