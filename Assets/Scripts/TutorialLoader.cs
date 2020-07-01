using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialLoader : MonoBehaviour {

    public static bool willReturnToMenu;

    public void LoadTutorial(string name) {
        willReturnToMenu = true;
        SceneManager.LoadScene(name);
    }
}
