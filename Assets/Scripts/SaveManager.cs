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
        completedTutorials = val;
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
        PlayerPrefs.DeleteAll();
        SetFinishedMap(PlayerPrefs.GetString("CompletedMaps", "0-0"));
    }

}
