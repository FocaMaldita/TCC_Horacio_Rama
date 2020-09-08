using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoopInfo : MonoBehaviour {
    public int origin;
    public int amount;

    private Text text;

    private void Start() {
        text = GetComponentInChildren<Text>();
    }

    private void Update() {
        if (amount > 0) text.text = "" + (amount + 1);
    }
}