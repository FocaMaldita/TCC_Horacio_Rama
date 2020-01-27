using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggedInstruction : MonoBehaviour {

    public Utils.InstructionType type;

    void Update() {
        transform.position = Vector3.Lerp(transform.position,
                                          Input.mousePosition,
                                          .6f);
    }

    public void onStartDrag() {
        Debug.Log("blah");
        Canvas canvas = FindObjectOfType<Canvas>();
        DragDropManager.currentDragItem = Instantiate(gameObject,
                                      transform.position,
                                      Quaternion.identity,
                                      canvas.transform
                                      );
        Destroy(DragDropManager.currentDragItem.GetComponent<InstructionListNode>());

        var list = transform.parent.parent.parent.GetComponent<InstructionList>();
        list.list.Remove(gameObject);
        Destroy(gameObject);
    }
}
