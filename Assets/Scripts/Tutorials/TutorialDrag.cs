using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TutorialDrag : MonoBehaviour {

    public Button moverButton, playButton;
    public RectTransform arrow, instruction, targetPlace, playButtonPlace;
    public InstructionButtonClick moverButtonClick;
    public GameObject instantiatedInstruction;
    public Transform catTransform;
    public Image fade;
    public EventSystem eventSystem;

    IEnumerator upAndDown(int times) {
        for (int i = 0; i < times; i++) {
            arrow.position = arrow.position - new Vector3(0, 10, 0);
            yield return new WaitForSeconds(.2f);
            arrow.position = arrow.position + new Vector3(0, 10, 0);
            yield return new WaitForSeconds(.2f);
        }
        arrow.position = arrow.position - new Vector3(0, 10, 0);
        yield return new WaitForSeconds(.2f);
    }

    IEnumerator fadeOut() {
        for (int i = 0; i < 10; i++) {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, i / 10f);
            yield return new WaitForSeconds(.1f);
        }
        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1f);
    }

    IEnumerator fadeIn() {
        for (int i = 0; i < 10; i++) {
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 1 - i / 10f);
            yield return new WaitForSeconds(.1f);
        }
        fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, 0f);
    }

    IEnumerator tutorialEvent() {
        while (true) {
            yield return new WaitForSeconds(2f);
            var moverButtonPosition = moverButton.GetComponent<RectTransform>().position + new Vector3(0, 75, 0);
            arrow.position = moverButtonPosition;
            arrow.gameObject.SetActive(true);
            yield return upAndDown(4);
            moverButton.Select();
            yield return new WaitForSeconds(.5f);
            eventSystem.SetSelectedGameObject(null);
            moverButtonClick.Extend();
            arrow.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            arrow.gameObject.SetActive(true);
            var instructionPosition = instruction.position + new Vector3(0, 75, 0);
            var targetPlacePosition = targetPlace.position;
            arrow.position = instructionPosition;
            yield return upAndDown(2);
            yield return new WaitForSeconds(1f);
            var arrowParent = arrow.parent;
            var instance = Instantiate(instantiatedInstruction, instruction.position, Quaternion.identity, targetPlace.parent);
            arrow.SetParent(instance.transform);
            yield return Utils.LerpObject(instance.GetComponent<RectTransform>(), instructionPosition, targetPlacePosition, 1f);
            yield return new WaitForSeconds(1f);
            arrow.SetParent(arrowParent);
            arrow.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);

            arrow.gameObject.SetActive(true);
            var playButtonPosition = playButtonPlace.position + new Vector3(0, 75, 0);
            arrow.position = playButtonPosition;
            yield return upAndDown(4);
            playButton.Select();
            yield return new WaitForSeconds(.5f);
            eventSystem.SetSelectedGameObject(null);
            yield return new WaitForSeconds(1f);
            yield return Utils.LerpObject(catTransform, catTransform.position, catTransform.position + new Vector3(0, 2, 0), .2f);
            arrow.gameObject.SetActive(false);

            yield return new WaitForSeconds(2f);

            yield return fadeOut();
            Destroy(instance);
            catTransform.position -= new Vector3(0, 2, 0);
            moverButtonClick.Extend();
            yield return new WaitForSeconds(1f);
            yield return fadeIn();
        }
    }

    public void toMainMenu() {
        SceneManager.LoadScene("Scenes/PuzzleMenu");
    }

    void Start() {
        StartCoroutine(tutorialEvent());
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            toMainMenu();
        }
    }
}
