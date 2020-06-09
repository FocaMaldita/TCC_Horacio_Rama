using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    [SerializeField]
    private GameObject menu, endMenu;
    [SerializeField]
    private Interpreter interpreter;

    public void openPauseMenu() {
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    public void closePauseMenu() {
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    public void toMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Scenes/PuzzleMenu");
    }

    public void restart(PuzzleManager puzzleManager) {
        puzzleManager.instantiatePuzzleMatrix();
        interpreter.resetMissionState();
        endMenu.SetActive(false);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menu.activeSelf) {
                closePauseMenu();
            } else {
                openPauseMenu();
            }
        }
    }
}
