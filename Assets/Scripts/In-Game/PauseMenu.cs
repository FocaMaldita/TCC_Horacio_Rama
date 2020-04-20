using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
    [SerializeField]
    private GameObject menu;

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
