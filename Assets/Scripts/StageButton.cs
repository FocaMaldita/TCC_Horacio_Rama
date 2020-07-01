using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour {

    public string requiredStage;

    private void Start() {
        if (!SaveManager.WasCompleted(requiredStage))
            GetComponent<Button>().interactable = false;
    }
}