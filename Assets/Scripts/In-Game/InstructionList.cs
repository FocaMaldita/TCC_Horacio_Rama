using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InstructionList : MonoBehaviour {

    public List<GameObject> list;

    [SerializeField]
    private GameObject instPrefab;

    private GameObject toBeAdded;

    public void insertAtPosition(int position) {
        list.Remove(null);
        if (position > list.Count) position = list.Count;
        if (toBeAdded != null) {
            var father = GameObject.Find("Node" + position).transform;
            if (father.childCount > 0) Destroy(father.GetChild(0));
            var go = Instantiate(instPrefab,
                             Utils.findChild(gameObject, "Node" + position).transform
                             );
            var node = go.GetComponent<InstructionListNode>();
            node.instructionList = this;
            node.index = position;
            node.instructionType = toBeAdded.GetComponent<DraggedInstruction>().type;

            var image = go.GetComponent<Image>();
            var draggedImage = toBeAdded.GetComponent<Image>();
            var color = draggedImage.color;
            color.a = 1;
            image.color = color;

            var icon = go.transform.GetChild(0).GetComponent<Text>();
            var draggedIcon = toBeAdded.transform.GetChild(0).GetComponent<Text>();
            icon.text = draggedIcon.text;
            node.dragItemIcon = icon.text;

            go.transform.GetChild(0).GetComponent<Text>().enabled = true;
            go.transform.localPosition = Vector2.zero;

            list.Insert(position, go);
        }
    }

    public void hoverPositionEnter(int position) {
        if (DragDropManager.currentDragItem != null) {
            list.Remove(null);
            if (position > list.Count) position = list.Count;
            list.Insert(position,
                        null
                        );
            toBeAdded = DragDropManager.currentDragItem;
        }
    }

    public void hoverPositionExit() {
        list.Remove(null);
    }
}
