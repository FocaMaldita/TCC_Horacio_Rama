using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {

    private static string completedMaps;
    private static int completedTutorials;

    public static string GetLastFinishedMap() {
        return completedMaps;
    }

    public static void SetFinishedMap(string val) {
        if (completedMaps != null && WasCompleted(val)) {
            return;
        }
        PlayerPrefs.SetString("CompletedMaps", val);
        PlayerPrefs.Save();
        completedMaps = val;
    }

    public static int GetLastFinishedTutorial() {
        return completedTutorials;
    }

    public static void SetFinishedTutorial(int val) {
        if (val <= GetLastFinishedTutorial()) {
            return;
        }
        PlayerPrefs.SetInt("CompletedTutorials", val);
        PlayerPrefs.Save();
        completedTutorials = val;
    }

    public static void DeleteAll() {
        completedMaps = "0-0";
        completedTutorials = 0;
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public static bool WasCompleted(string val) {
        var stage = val.Split('-');
        var completedStage = completedMaps.Split('-');
        if (int.Parse(completedStage[0]) < int.Parse(stage[0])) {
            return false;
        } else if (int.Parse(completedStage[0]) > int.Parse(stage[0])) {
            return true;
        } else {
            return int.Parse(completedStage[1]) >= int.Parse(stage[1]);
        }
    }

    private void Awake() {
        SetFinishedMap(PlayerPrefs.GetString("CompletedMaps", "0-0"));
        SetFinishedTutorial(PlayerPrefs.GetInt("CompletedTutorials", 0));
    }

}
