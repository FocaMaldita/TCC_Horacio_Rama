using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentInstructionArrow : MonoBehaviour {

    public int target;
    public float duration = .5f;

    [SerializeField]
    private RectTransform[] targetRects;

    private RectTransform rectTransform;
    private Vector3 initialPosition;

    private float time = 0;

    IEnumerator spawnAnimation() {
        for (int i=0; i < 10; i++) {
            rectTransform.localScale = new Vector3((i + 1) / 10f, (i + 1) / 10f, 1f);
            yield return new WaitForSeconds(duration / 10f);
        }
    }

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.localPosition - targetRects[target].localPosition;
    }

    private void OnEnable() {
        target = 0;
        rectTransform.localPosition = initialPosition;
        StartCoroutine(spawnAnimation());
    }

    private void Update() {
        if (time > .05f) {
            time = 0;
            rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition,
                                targetRects[target].localPosition + initialPosition,
                                .5f);
        }
        time += Time.deltaTime;
    }
}
