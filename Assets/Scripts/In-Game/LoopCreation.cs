using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopCreation : MonoBehaviour {

    public GameObject[] loopPrefabs;
    public RectTransform[] instructionSlots;
    public GameObject loopSizeMenu;

    private RectTransform arrowTransform, curveTransform, verticalStretchTransform;
    private float bottomPosition, topPosition, initialTopPosition;
    private float topLocalPosition, initialTopLocalPosition;
    private float verticalStretchInitialHeight;
    private GameObject[] loops;
    private LoopInfo[] loopInfos;
    private Image[] images;

    private bool isValid = true;

    private float[] maximumHeights;

    private LoopInfo loopInfoBeingModified;

    private void applyPositions() {
        arrowTransform.position = new Vector3(
                                    arrowTransform.position.x,
                                    topPosition,
                                    arrowTransform.position.z
                                    );
        curveTransform.position = new Vector3(
                                    curveTransform.position.x,
                                    topPosition,
                                    curveTransform.position.z
                                    );

        topLocalPosition = arrowTransform.localPosition.y;
        verticalStretchTransform.sizeDelta = new Vector2(verticalStretchTransform.rect.width, topLocalPosition - initialTopLocalPosition + 1f);
    }

    private bool loopCanBeCreated(int endingNode) {
        if (topPosition < bottomPosition)
            return false;
        int attempted_origin = 0;
        for (int i = 0; i < maximumHeights.Length; i++) {
            if (topPosition > maximumHeights[i]) break;
            attempted_origin = i;
        }
        if (attempted_origin == endingNode) { // loop of size 1 can't intertwine
            return true;
        }
        for (int i = attempted_origin; i < endingNode; i++) { // check if there's any node inbetween pointing to before attempted_origin
            if (loopInfos[i] && loopInfos[i].origin < attempted_origin)
                return false;
        }
        for (int i = endingNode + 1; i < instructionSlots.Length; i++) { // check if there's any node starting after endingNode and pointing to something inbetween
            if (loopInfos[i] && loopInfos[i].origin > attempted_origin && loopInfos[i].origin <= endingNode)
                return false;
        }
        return true;
    }

    public void onStartDrag(int endingNode) {
        if (!Interpreter.isInterpreting) {
            if (loops[endingNode]) {
                Destroy(loops[endingNode]);
            }
            loops[endingNode] = Instantiate(loopPrefabs[0],
                                        instructionSlots[endingNode].transform.position,
                                        Quaternion.identity,
                                        instructionSlots[endingNode]
                                        );
            arrowTransform = Utils.findChild(loops[endingNode], "Arrow").GetComponent<RectTransform>();
            curveTransform = Utils.findChild(loops[endingNode], "TopCurve").GetComponent<RectTransform>();
            verticalStretchTransform = Utils.findChild(loops[endingNode], "VerticalStretch").GetComponent<RectTransform>();
            verticalStretchInitialHeight = verticalStretchTransform.rect.height;
            bottomPosition = Utils.findChild(loops[endingNode], "Base").GetComponent<RectTransform>().position.y;

            topPosition = arrowTransform.GetComponent<RectTransform>().position.y;
            initialTopPosition = topPosition;
            topLocalPosition = arrowTransform.GetComponent<RectTransform>().localPosition.y;
            initialTopLocalPosition = topLocalPosition;

            images = loops[endingNode].GetComponentsInChildren<Image>();
        }
    }

    public void duringDrag(int endingNode) {
        if (arrowTransform) {
            topPosition = Mathf.Lerp(arrowTransform.position.y,
                            Input.mousePosition.y,
                            .6f);
            if (!loopCanBeCreated(endingNode)) {
                isValid = false;
                foreach(Image image in images) {
                    image.color = new Color(
                        image.color.r,
                        image.color.g,
                        image.color.b,
                        .5f
                    );
                }
            } else {
                isValid = true;
                foreach (Image image in images) {
                    image.color = new Color(
                        image.color.r,
                        image.color.g,
                        image.color.b,
                        1f
                    );
                }
            }
            if (topPosition < initialTopPosition) {
                topPosition = initialTopPosition;
            }
            if (topPosition > maximumHeights[0]) {
                topPosition = maximumHeights[0];
            }

            applyPositions();
        }
    }

    public void onEndDrag(int endingNode) {
        if (!isValid) {
            Destroy(loops[endingNode]);
        } else {
            loopInfos[endingNode] = loops[endingNode].GetComponent<LoopInfo>();
            loopInfoBeingModified = loopInfos[endingNode];
            loopInfoBeingModified.origin = 0;
            float willSnapTo = maximumHeights[0];
            for (int i = 0; i < maximumHeights.Length; i++) {
                if (topPosition > maximumHeights[i]) break;
                willSnapTo = maximumHeights[i];
                loopInfoBeingModified.origin = i;
            }
            topPosition = willSnapTo;
            applyPositions();
            loopSizeMenu.SetActive(true);
        }
    }

    public void chooseAmount(int amount) {
        LoopInfo info = loopInfoBeingModified;
        info.amount = amount;
        loopSizeMenu.SetActive(false);
    }

    private void Start() {
        maximumHeights = new float[instructionSlots.Length];
        loops = new GameObject[instructionSlots.Length];
        loopInfos = new LoopInfo[instructionSlots.Length];
        for (int i = 0; i < instructionSlots.Length; i++) {
            maximumHeights[i] = instructionSlots[i].position.y + instructionSlots[i].rect.height / 2;
        }
    }
}
