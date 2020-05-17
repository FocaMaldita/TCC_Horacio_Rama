using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectController : MonoBehaviour {

    public GameObject[] groups;

    public Button leftButton, rightButton;

    public float transitionDuration = 1;

    private int currentGroup = 0;

    void Start() {
        disableButton(leftButton);
        if (groups.Length < 2) {
            disableButton(rightButton);
        }

        for (int i = 1; i < groups.Length; i++) {
            groups[i].SetActive(false);
        }
    }

    void enableButton(Button button) {
        button.enabled = true;
        button.transform.GetChild(0).GetComponent<Image>().color = Color.white;
    }

    void disableButton(Button button) {
        button.enabled = false;
        button.transform.GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, .5f);
    }

    public void Paginate(int pages) {
        int destination = currentGroup + pages;
        if (destination < 0) {
            destination = 0;
        } else if (destination >= groups.Length) {
            destination = groups.Length - 1;
        }

        if (currentGroup == destination) {
            return;
        }

        if (currentGroup < destination) {
            StartCoroutine(PaginateRight(destination));
        }

        if (currentGroup > destination) {
            StartCoroutine(PaginateLeft(destination));
        }
    }

    public IEnumerator PaginateRight(int destination) {
        disableButton(leftButton);
        disableButton(rightButton);

        groups[destination].SetActive(true);
        groups[destination].transform.localPosition = new Vector3(1600, 0, 0);

        for (int i = 0; i < 30; i++) {
            groups[destination].transform.localPosition = new Vector3(1600 - i * (1600 / 30), 0, 0);
            groups[currentGroup].transform.localPosition = new Vector3(0 - i * (1600 / 30), 0, 0);
            yield return new WaitForSeconds(transitionDuration / 30);
        }

        groups[destination].transform.localPosition = Vector3.zero;

        groups[currentGroup].SetActive(false);
        currentGroup = destination;

        if (destination > 0) {
            enableButton(leftButton);
        }
        if (destination < groups.Length - 1) {
            enableButton(rightButton);
        }
    }

    public IEnumerator PaginateLeft(int destination) {
        disableButton(leftButton);
        disableButton(rightButton);

        groups[destination].SetActive(true);
        groups[destination].transform.localPosition = new Vector3(-1600, 0, 0);

        for (int i = 0; i < 30; i++) {
            groups[destination].transform.localPosition = new Vector3(-1600 + i * (1600 / 30), 0, 0);
            groups[currentGroup].transform.localPosition = new Vector3(0 + i * (1600 / 30), 0, 0);
            yield return new WaitForSeconds(transitionDuration / 30);
        }

        groups[destination].transform.localPosition = Vector3.zero;

        groups[currentGroup].SetActive(false);
        currentGroup = destination;

        if (destination > 0) {
            enableButton(leftButton);
        }
        if (destination < groups.Length - 1) {
            enableButton(rightButton);
        }
    }
}
