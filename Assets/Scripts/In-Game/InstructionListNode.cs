using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionListNode : MonoBehaviour {

    public int index;

    public InstructionList instructionList;
    
    void Update() {
        index = instructionList.list.FindIndex(obj => obj == gameObject);
        Debug.Log(transform.parent.name);
        Debug.Log("Node" + index);
        Debug.Log(string.Compare(transform.parent.name, "Node" + index));
        if (string.Compare(transform.parent.name, "Node" + index) != 0) {
            Debug.Log(transform.name + " (" + index + "): " + transform.parent.name);
            transform.parent = transform.parent.parent.Find("Node" + index);
            Debug.Log(transform.name + " (" + index + "): " + transform.parent.name);
        }
        transform.position = Vector3.Lerp(transform.position,
                                          transform.parent.position,
                                          .4f
                                          );
    }
}
