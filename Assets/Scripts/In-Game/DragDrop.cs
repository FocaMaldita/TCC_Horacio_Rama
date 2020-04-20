using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DragDrop : MonoBehaviour {

    public Utils.InstructionType instruction;

    [SerializeField]
    private GameObject dragItem;

    [SerializeField]
    private Sprite dragItemIcon;
    [SerializeField]
    private float rotation;

    private Color color;

    public void Start() {
        color = GetComponent<Image>().color;
        color.a = .5f;
    }

    public void onStartDrag() {
        if (!Interpreter.isInterpreting) {
            Canvas canvas = FindObjectOfType<Canvas>();
            DragDropManager.currentDragItem = Instantiate(dragItem,
                                          transform.position,
                                          Quaternion.identity,
                                          canvas.transform
                                          );
            DragDropManager.currentDragItem.GetComponent<DraggedInstruction>().type = instruction;
            var img = DragDropManager.currentDragItem.GetComponent<Image>();
            img.color = color;
            img.sprite = GetComponent<Image>().sprite;
            var childImage = DragDropManager.currentDragItem.transform.GetChild(0).GetComponent<Image>();
            childImage.sprite = dragItemIcon;
            childImage.transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }
}
