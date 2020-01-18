using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionList : MonoBehaviour {

    public List<GameObject> list;

    private GameObject toBeAdded;

    public void insertAtPosition(int position) {
        list.Remove(null);
        if (position > list.Count) position = list.Count;
        if (toBeAdded != null) {
            var father = GameObject.Find("Node" + position).transform;
            if (father.childCount > 0) Destroy(father.GetChild(0));
            var go = Instantiate(toBeAdded,
                             GameObject.Find("Node" + position).transform
                             //transform.FindChild("Node" + position)
                             );
            go.GetComponent<DraggedInstruction>().enabled = false;
            var image = go.GetComponent<Image>();
            var color = image.color;
            color.a = 1;
            image.enabled = true;
            image.color = color;
            var node = go.AddComponent<InstructionListNode>();
            node.index = position;
            node.instructionList = this;
            go.transform.GetChild(0).GetComponent<Text>().enabled = true;
            go.transform.localPosition = Vector2.zero;
            list.Insert(position, go);
        }
    }

    public void hoverPositionEnter(int position) {
        if (DragDrop.currentDragItem != null) {
            toBeAdded = DragDrop.currentDragItem;
            list.Remove(null);
            if (position > list.Count) position = list.Count;
            list.Insert(position,
                        null
                        );
        }
    }

    public void hoverPositionExit() {
        list.Remove(null);
    }
}
