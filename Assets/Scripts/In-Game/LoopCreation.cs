﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopCreation : MonoBehaviour {

    public GameObject[] loopPrefabs;
    public RectTransform[] instructionSlots;
    public GameObject loopSizeMenu;

    private RectTransform arrowTransform, curveTransform, topStretchTransform, verticalStretchTransform;
    private float bottomPosition, topPosition, initialTopPosition;
    private float topLocalPosition, initialTopLocalPosition;
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
        topStretchTransform.position = new Vector3(
                                    topStretchTransform.position.x,
                                    topPosition,
                                    topStretchTransform.position.z
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

    private void resetAllCounts() {
        for (int i = 0; i < loopInfos.Length; i++) {
            if (loopInfos[i]) {
                loopInfos[i].howManyAbove = 0;
                loopInfos[i].howManyInside = 0;
            }
        }
    }

    private int countInnerLoops(int endingNode, int new_loop, int origin = -1) {
        int inner_loops = 0;
        int new_loop_origin = 0;
        if (origin == -1) {
            origin = 0;
            for (int i = 0; i < maximumHeights.Length; i++) {
                if (topPosition > maximumHeights[i]) break;
                origin = i;
            }
        } else {
            new_loop_origin = 0;
            for (int i = 0; i < maximumHeights.Length; i++) {
                if (topPosition > maximumHeights[i]) break;
                new_loop_origin = i;
            }
        }
        for (int i = origin; i < endingNode; i++) {
            if (i == new_loop) {
                if (loopInfos[i] && isValid && new_loop_origin >= origin) {
                    if (loopInfos[i].howManyInside + 1 > inner_loops) inner_loops = loopInfos[i].howManyInside + 1;
                }
            } else {
                if (loopInfos[i] && loopInfos[i].origin >= origin) {
                    if (loopInfos[i].howManyInside + 1 > inner_loops) inner_loops = loopInfos[i].howManyInside + 1;
                }
            }
        }
        return inner_loops;
    }

    private int countParentLoops(int endingNode, int new_loop, int origin = -1) {
        int parent_loops = 0;
        int new_loop_origin = 0;
        if (origin == -1) {
            origin = 0;
            for (int i = 0; i < maximumHeights.Length; i++) {
                if (topPosition > maximumHeights[i]) break;
                origin = i;
            }
        } else {
            new_loop_origin = 0;
            for (int i = 0; i < maximumHeights.Length; i++) {
                if (topPosition > maximumHeights[i]) break;
                new_loop_origin = i;
            }
        }
        for (int i = endingNode + 1; i < instructionSlots.Length; i++) {
            if (i == new_loop) {
                if (loopInfos[i] && isValid && new_loop_origin == origin) {
                    parent_loops++;
                }
            } else {
                if (loopInfos[i] && loopInfos[i].origin == origin) {
                    parent_loops++;
                }
            }
        }
        return parent_loops;
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
            loopInfos[endingNode] = loops[endingNode].GetComponent<LoopInfo>();
            arrowTransform = Utils.findChild(loops[endingNode], "Arrow").GetComponent<RectTransform>();
            curveTransform = Utils.findChild(loops[endingNode], "TopCurve").GetComponent<RectTransform>();
            verticalStretchTransform = Utils.findChild(loops[endingNode], "VerticalStretch").GetComponent<RectTransform>();
            topStretchTransform = Utils.findChild(loops[endingNode], "TopStretch").GetComponent<RectTransform>();
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
            resetAllCounts();
            for (int i = 0; i < loopInfos.Length; i++) {
                if (i == endingNode) {
                    loopInfos[endingNode].howManyAbove = countParentLoops(endingNode, endingNode);
                    loopInfos[endingNode].howManyInside = countInnerLoops(endingNode, endingNode);
                } else {
                    if (loopInfos[i]) {
                        loopInfos[i].howManyAbove = countParentLoops(i, endingNode, loopInfos[i].origin);
                        loopInfos[i].howManyInside = countInnerLoops(i, endingNode, loopInfos[i].origin);
                    }
                }
            }
        }
    }

    public void onEndDrag(int endingNode) {
        if (!isValid) {
            Destroy(loops[endingNode]);
        } else {
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
            loopInfos[endingNode].initialCurveY = arrowTransform.localPosition.y;
            loopSizeMenu.SetActive(true);
        }
        resetAllCounts();
        for (int i = 0; i < loopInfos.Length; i++) {
            if (loopInfos[i]) {
                loopInfos[i].howManyAbove = countParentLoops(i, endingNode, loopInfos[i].origin);
                loopInfos[i].howManyInside = countInnerLoops(i, endingNode, loopInfos[i].origin);
            }
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
