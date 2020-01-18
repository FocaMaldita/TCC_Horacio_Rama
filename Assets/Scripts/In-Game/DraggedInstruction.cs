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
}
