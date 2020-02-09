using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionListHover : MonoBehaviour {

    public InstructionList instructionList;

    void Update() {

        if (instructionList.list.Contains(null)) {
            var nullPosition = instructionList.list.FindIndex(obj => obj == null);
            transform.position = Utils.findChild(instructionList.gameObject,
                "Node" + nullPosition).transform.position ;
            if (transform.childCount == 0) {
                var obj = Instantiate(DragDropManager.currentDragItem, transform);
                obj.transform.localPosition = Vector3.zero;
                obj.GetComponent<DraggedInstruction>().enabled = false;
            }
        } else {
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
        
    }
}
