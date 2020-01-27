using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragDrop : MonoBehaviour {

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
        DragDropManager.currentDragItem = Instantiate(dragItem,
                                      transform.position,
                                      Quaternion.identity,
                                      canvas.transform
                                      );
        DragDropManager.currentDragItem.GetComponent<DraggedInstruction>().type = instruction;
        DragDropManager.currentDragItem.GetComponent<Image>().color = color;
        DragDropManager.currentDragItem.GetComponentInChildren<Text>().text = dragItemIcon;
    }
}
