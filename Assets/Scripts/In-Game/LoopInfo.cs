using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopInfo : MonoBehaviour {
    public int origin = -1;
    public int amount;

    public int howManyInside = 0;
    public int howManyAbove = 0;

    private Text text;
    private RectTransform arrowTransform, topCurveTransform, bottomCurveTransform, topStretchTransform, bottomStretchTransform, verticalStretchTransform;
    private float initialCurveX;
    public float initialCurveY = 0;

    private void Start() {
        text = GetComponentInChildren<Text>();
        arrowTransform = Utils.findChild(gameObject, "Arrow").GetComponent<RectTransform>();
        topCurveTransform = Utils.findChild(gameObject, "TopCurve").GetComponent<RectTransform>();
        bottomCurveTransform = Utils.findChild(gameObject, "BottomCurve").GetComponent<RectTransform>();
        topStretchTransform = Utils.findChild(gameObject, "TopStretch").GetComponent<RectTransform>();
        bottomStretchTransform = Utils.findChild(gameObject, "BottomStretch").GetComponent<RectTransform>();
        verticalStretchTransform = Utils.findChild(gameObject, "VerticalStretch").GetComponent<RectTransform>();
        initialCurveX = topCurveTransform.localPosition.x;
    }

    private void Update() {
        if (amount > 0) text.text = "" + (amount + 1);
        bottomCurveTransform.localPosition = new Vector3(Mathf.Lerp(bottomCurveTransform.localPosition.x,
                                                                   initialCurveX - 20 * howManyInside,
                                                                   .6f),
                                                         bottomCurveTransform.localPosition.y,
                                                         bottomCurveTransform.localPosition.z);

        verticalStretchTransform.localPosition = new Vector3(Mathf.Lerp(verticalStretchTransform.localPosition.x,
                                                                        initialCurveX - 20 * howManyInside,
                                                                        .6f),
                                                             verticalStretchTransform.localPosition.y,
                                                             verticalStretchTransform.localPosition.z);

        topStretchTransform.sizeDelta = new Vector2(20 * howManyInside, topStretchTransform.rect.height);
        bottomStretchTransform.sizeDelta = new Vector2(20 * howManyInside, bottomStretchTransform.rect.height);

        if (initialCurveY != 0) {
            topCurveTransform.localPosition = new Vector3(Mathf.Lerp(topCurveTransform.localPosition.x,
                                                                     initialCurveX - 20 * howManyInside,
                                                                     .6f),
                                                          Mathf.Lerp(topCurveTransform.localPosition.y,
                                                                     initialCurveY - 5 * howManyAbove,
                                                                     .6f),
                                                          topCurveTransform.localPosition.z);

            topStretchTransform.localPosition = new Vector3(topStretchTransform.localPosition.x,
                                                          Mathf.Lerp(topStretchTransform.localPosition.y,
                                                                     initialCurveY - 5 * howManyAbove,
                                                                     .6f),
                                                          topStretchTransform.localPosition.z);

            arrowTransform.localPosition = new Vector3(arrowTransform.localPosition.x,
                                                          Mathf.Lerp(arrowTransform.localPosition.y,
                                                                     initialCurveY - 5 * howManyAbove,
                                                                     .6f),
                                                          arrowTransform.localPosition.z);
        } else {
            topCurveTransform.localPosition = new Vector3(Mathf.Lerp(topCurveTransform.localPosition.x,
                                                                     initialCurveX - 20 * howManyInside,
                                                                     .6f),
                                                          topCurveTransform.localPosition.y,
                                                          topCurveTransform.localPosition.z);
        }
    }
}