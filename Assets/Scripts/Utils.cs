using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    public enum InstructionType {
        MOVE_U,
        MOVE_L,
        MOVE_R,
        MOVE_D,
        GRAB_U,
        GRAB_L,
        GRAB_R,
        GRAB_D,
        WAIT
    };

    public static char CanCatMove(PuzzleManager.PuzzleObject obstacle) {
        // Is blocked by
        if ((new List<PuzzleManager.PuzzleObject> {
            PuzzleManager.PuzzleObject.ROCK,
            PuzzleManager.PuzzleObject.DOG,
            PuzzleManager.PuzzleObject.BIRD,
            // TODO
        }).Contains(obstacle)) {
            return 'N';
        }

        // Can push
        if ((new List<PuzzleManager.PuzzleObject> {
            // TODO
        }).Contains(obstacle)) {
            return 'P';
        }

        // Can move
        return 'Y';
    }

    public static char CanCatGrab(PuzzleManager.PuzzleObject obstacle) {
        // Can grab
        if ((new List<PuzzleManager.PuzzleObject> {
            PuzzleManager.PuzzleObject.BIRD,
            // TODO
        }).Contains(obstacle)) {
            return 'Y';
        }

        // Can't grab
        return 'N';
    }

    public static char CanDogMove(PuzzleManager.PuzzleObject obstacle) {
        // Is blocked by
        if ((new List<PuzzleManager.PuzzleObject> {
            PuzzleManager.PuzzleObject.CAT,
            PuzzleManager.PuzzleObject.BIRD,
            // TODO
        }).Contains(obstacle)) {
            return 'N';
        }

        // Can push
        if ((new List<PuzzleManager.PuzzleObject> {
            PuzzleManager.PuzzleObject.ROCK,
            // TODO
        }).Contains(obstacle)) {
            return 'P';
        }

        // Can move
        return 'Y';
    }

    public static char CanDogGrab(PuzzleManager.PuzzleObject obstacle) {
        // Can grab
        if ((new List<PuzzleManager.PuzzleObject> {
            PuzzleManager.PuzzleObject.BIRD,
            PuzzleManager.PuzzleObject.CAT,
            // TODO
        }).Contains(obstacle)) {
            return 'Y';
        }

        // Can't grab
        return 'N';
    }

    public static char CanPlaceObject(PuzzleManager.PuzzleObject obj) {
        // Can place
        if ((new List<PuzzleManager.PuzzleObject> {
            PuzzleManager.PuzzleObject.NTH,
            // TODO
        }).Contains(obj)) {
            return 'Y';
        }

        // Can't place
        return 'N';
    }

    public static bool IsCatInUpperCorner(PuzzleManager puzzleManager) {
        return puzzleManager.catPosition[1] == puzzleManager.kindMatrix.GetLength(1) - 1;
    }

    public static bool IsCatInLeftCorner(PuzzleManager puzzleManager) {
        return puzzleManager.catPosition[0] == 0;
    }

    public static bool IsCatInRightCorner(PuzzleManager puzzleManager) {
        return puzzleManager.catPosition[0] == puzzleManager.kindMatrix.GetLength(0) - 1;
    }

    public static bool IsCatInBottomCorner(PuzzleManager puzzleManager) {
        return puzzleManager.catPosition[1] == 0;
    }

    public static bool IsDogInUpperCorner(PuzzleManager puzzleManager) {
        return puzzleManager.dogPosition[1] == puzzleManager.kindMatrix.GetLength(1) - 1;
    }

    public static bool IsDogInLeftCorner(PuzzleManager puzzleManager) {
        return puzzleManager.dogPosition[0] == 0;
    }

    public static bool IsDogInRightCorner(PuzzleManager puzzleManager) {
        return puzzleManager.dogPosition[0] == puzzleManager.kindMatrix.GetLength(0) - 1;
    }

    public static bool IsDogInBottomCorner(PuzzleManager puzzleManager) {
        return puzzleManager.dogPosition[1] == 0;
    }

    public static IEnumerator cooldown (float duration, System.Action callback) {
        yield return new WaitForSeconds(duration);
        callback();
    }

    public static GameObject findChild(GameObject go, string childName) {
        foreach (Transform trans in go.GetComponentsInChildren<Transform>()) {
            if (trans.name == childName) {
                return trans.gameObject;
            }
        }
        return null;
    }
}
