using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionListNode : MonoBehaviour {

    [HideInInspector]
    public int index = 0;

    public InstructionList instructionList;

    [SerializeField]
    public Utils.InstructionType instructionType;

    [SerializeField]
    private GameObject dragItem;

    [SerializeField]
    public string dragItemIcon;

    private Image img;
    private Color color;

    void Start() {
        img = GetComponent<Image>();
        color = img.color;
        color.a = .5f;
    }

    void Update() {
        index = instructionList.list.FindIndex(obj => obj == gameObject);
        if (string.Compare(transform.parent.name, "Node" + index) != 0) {
            transform.SetParent(transform.parent.parent.Find("Node" + index));
        }
        transform.position = Vector3.Lerp(transform.position,
                                          transform.parent.position,
                                          .4f
                                          );
        if (DragDropManager.currentDragItem && img.raycastTarget) {
            img.raycastTarget = false;
        } else if (!DragDropManager.currentDragItem && !img.raycastTarget) {
            img.raycastTarget = true;
        }
    }

    public void onStartDrag() {
        Canvas canvas = FindObjectOfType<Canvas>();
        DragDropManager.currentDragItem = Instantiate(dragItem,
                                      transform.position,
                                      Quaternion.identity,
                                      canvas.transform
                                      );
        DragDropManager.currentDragItem.GetComponent<DraggedInstruction>().type = instructionType;
        DragDropManager.currentDragItem.GetComponent<Image>().color = color;
        DragDropManager.currentDragItem.GetComponentInChildren<Text>().text = dragItemIcon;

        instructionList.list.Remove(gameObject);
        Destroy(gameObject);
    }

    public void onFinishDrag() {
        FindObjectOfType<DragDropManager>().onFinishDrag();
    }
}
