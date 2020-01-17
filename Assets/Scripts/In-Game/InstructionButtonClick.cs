using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionButtonClick : MonoBehaviour {

    public static InstructionButtonClick whoIsOpen = null;

    [HideInInspector]
    public bool isOpen = false;
    public RectTransform extendable;

    private Animator extAnim;

    public void Extend() {
        isOpen = !isOpen;
        if (isOpen && whoIsOpen != this) {
            if (whoIsOpen == null) {
                whoIsOpen = this;
            } else if (whoIsOpen.isOpen) {
                whoIsOpen.Extend();
                whoIsOpen = this;
            }
        } else if (isOpen == false) {
            whoIsOpen = null;
        }
        extAnim.SetBool("isOpen", isOpen);
    }

    public void Start() {
        extAnim = extendable.GetComponent<Animator>();
    }
    public void Update() {
        Debug.Log("whoIsOpen:" + whoIsOpen);
    }
}
