using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionResult {

    public enum Condition {
        SUCCESS,

        CAT_WALK_FAIL,
        CAT_GRAB_FAIL,
        DOG_WALK_FAIL,
        DOG_GRAB_FAIL,
        CAT_DOG_WALK_INTO_EACH_OTHER_FAIL,
        CAT_DOG_GRAB_SAME_THING_FAIL,

        CAT_NOT_IN_GOAL_FAIL,
        DOG_NOT_IN_GOAL_FAIL,
        ANIMAL_NOT_IN_GOAL_FAIL,
        ITEM_NOT_IN_GOAL_FAIL,
    };

    public static Dictionary<Condition, string> conditionNames = new Dictionary<Condition, string> {
        { Condition.SUCCESS, "Sucesso!" },

        { Condition.CAT_WALK_FAIL, "Gato bateu na parede!" },
        { Condition.CAT_GRAB_FAIL, "Gato pegou/largou onde não deve!" },
        { Condition.DOG_WALK_FAIL, "Cachorra bateu na parede!" },
        { Condition.DOG_GRAB_FAIL, "Cachorra pegou/largou onde não deve!" },
        { Condition.CAT_DOG_WALK_INTO_EACH_OTHER_FAIL, "Gato e Cachorra se esbarraram!" },
        { Condition.CAT_DOG_GRAB_SAME_THING_FAIL, "" },

        { Condition.CAT_NOT_IN_GOAL_FAIL, "Gato não alcançou o objetivo!" },
        { Condition.DOG_NOT_IN_GOAL_FAIL, "Cachorra não alcançou o objetivo!" },
        { Condition.ANIMAL_NOT_IN_GOAL_FAIL, "Há animal(is) não resgatado(s)!" },
        { Condition.ITEM_NOT_IN_GOAL_FAIL, "Há item(ns) não resgatado(s)!" },
    };

    public static Condition checkMissionResult(PuzzleManager puzzleManager) {
        for (int i = 0; i < PuzzleManager.stageInfo.colCount; i++) {
            for (int j = 0; j < PuzzleManager.stageInfo.rowCount; j++) {
                if (PuzzleManager.stageInfo.matrix[i].entries[j] == PuzzleManager.PuzzleObject.GOAL_CAT
                        && (puzzleManager.catPosition[0] != i || puzzleManager.catPosition[1] != j)) {
                    return Condition.CAT_NOT_IN_GOAL_FAIL;
                }
                if (PuzzleManager.stageInfo.matrix[i].entries[j] == PuzzleManager.PuzzleObject.GOAL_DOG
                        && (puzzleManager.dogPosition[0] != i || puzzleManager.dogPosition[1] != j)) {
                    return Condition.DOG_NOT_IN_GOAL_FAIL;
                }
            }
        }
        return Condition.SUCCESS;
    }

}
