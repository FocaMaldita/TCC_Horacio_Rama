using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUIOrder : MonoBehaviour {
    public void changeOrderOfParentInGrandparent() {
        transform.parent.SetAsLastSibling();
    }
}
