using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialPagination : MonoBehaviour {

    public string puzzleToRedirectTo;
    public int puzzleNumber;
    public GameObject[] pages;
    public GameObject leftButton, rightButton, closeButton;
    [HideInInspector]
    public int currentPage = 0;

    public void paginate(int diff) {
        if (currentPage + diff >= 0 && currentPage + diff < pages.Length) {
            pages[currentPage].SetActive(false);
            currentPage += diff;
            pages[currentPage].SetActive(true);

            leftButton.SetActive(currentPage != 0);
            rightButton.SetActive(currentPage != pages.Length - 1);
        }

        if (currentPage == pages.Length - 1) {
            closeButton.SetActive(true);
        }
    }

    public void toMainMenu() {
        SaveManager.SetFinishedTutorial(puzzleNumber);
        if (TutorialLoader.willReturnToMenu) {
            SceneManager.LoadScene("Scenes/PuzzleMenu");
        } else {
            new PuzzleLoader().loadPuzzle(puzzleToRedirectTo);
        }
    }

    private void Start() {
        foreach (var page in pages) {
            page.SetActive(false);
        }
        pages[currentPage].SetActive(true);

        closeButton.SetActive(false);
        leftButton.SetActive(currentPage != 0);
        rightButton.SetActive(currentPage != pages.Length - 1);
    }
}
