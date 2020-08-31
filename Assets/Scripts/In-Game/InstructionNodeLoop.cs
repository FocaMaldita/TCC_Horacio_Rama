using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionNodeLoop : MonoBehaviour {
    public int origin;
    public int loopSize;

    public PuzzleManager puzzleManager;

    private InputField destiny, amount;
    private GameObject destinyObj, amountObj;

    public void recoverSize() {
        int amount_text;
        if (int.TryParse(amount.text, out amount_text)) {
            if (amount_text > 1 && amount_text < 6) {
                loopSize = amount_text - 1;
            } else {
                loopSize = 0;
            }
        } else {
            loopSize = 0;
        }
    }

    private void Start() {
        destinyObj = transform.Find("OrigemInputField").gameObject;
        amountObj = transform.Find("QtdeInputField").gameObject;
        destiny = destinyObj.GetComponent<InputField>();
        amount = amountObj.GetComponent<InputField>();
    }
    private void Update() {
        if (puzzleManager.phase == 1 && !Interpreter.isInterpreting) {
            destinyObj.SetActive(true);
            amountObj.SetActive(true);
            int destiny_text, amount_text;
            if (int.TryParse(destiny.text, out destiny_text)) {
                if (destiny_text > 1 && destiny_text < 6) {
                    origin = destiny_text - 1;
                } else {
                    origin = 0;
                }
            } else {
                origin = 0;
            }
            if (int.TryParse(amount.text, out amount_text)) {
                if (amount_text > 1 && amount_text < 6) {
                    loopSize = amount_text - 1;
                } else {
                    loopSize = 0;
                }
            } else {
                loopSize = 0;
            }
        } else {
            destinyObj.SetActive(false);
            amountObj.SetActive(false);
        }
    }
}
