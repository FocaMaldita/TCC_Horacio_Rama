using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PuzzleLoader : MonoBehaviour {

    public List<string> tutorials;

    public void loadPuzzle(string a) {
        if (tutorials != null) {
            // Check if needs to redirect player to appropriate tutorial
            var stage = int.Parse(a.Split('-')[0]);
            if (SaveManager.GetLastFinishedTutorial() < stage) {
                var index = stage - 1;
                if (index < tutorials.Count) {
                    TutorialLoader.willReturnToMenu = false;
                    SceneManager.LoadScene(tutorials[index]);
                    return;
                }
            }
        }
        Utils.loadPuzzle("PuzzleStage" + a);
    }
}
