
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {

    public static PuzzleStageScriptableObject stageInfo;

    public float cellDistance = .5f;

    public enum PuzzleObject {
        NTH=0,
        CAT=1,
        DOG=2,
        BIRD=11,
        EGG=12,
        PUPPER=13,
        SQUIRREL=14,
        BASKET=31,
        NEST=32,
        TREE_LOWER=41,
        TREE_UPPER=42,
        BLOCK=43,
        PUSHABLE=44,
        GOAL_CAT=51,
        GOAL_DOG=52,
        ANIMAL_POINT=53,
        ITEM_POINT=54,
        ANIMAL_POINT_SQUIRREL=61,
        ANIMAL_POINT_BIRD=62,
        ANIMAL_POINT_BIRD_X2=63,
        ANIMAL_POINT_BIRD_X3=64,
        ITEM_POINT_EGG=71,
        ITEM_POINT_EGG_X2=72,
        ITEM_POINT_EGG_X3=73,
        TREE_WITH_BIRD=81,
        TREE_WITH_BIRD_X2=82,
        TREE_WITH_BIRD_X3=83,
    }

    [System.Serializable]
    public struct PuzzleObjectPrefabMap{
        public PuzzleObject puzzleObject;
        public GameObject prefab;
    }
    
    public PuzzleObjectPrefabMap[] prefabList;
    [HideInInspector]
    public Dictionary<PuzzleObject, GameObject> prefabDict;

    [HideInInspector]
    public int[] catPosition, dogPosition;
    [HideInInspector]
    public GameObject catReference, dogReference;

    public PuzzleObject[,] kindMatrix;
    public GameObject[,] objMatrix;
    private int rowCount, colCount;

    [SerializeField]
    private GameObject catList, dogList;
    [SerializeField]
    private GameObject moveButton, grabButton, waitButton;

    [SerializeField]
    private Transform puzzleObjects;

    [System.Serializable]
    public struct ConditionPrefabMap {
        public string condition;
        public GameObject prefab;
    }

    [SerializeField]
    private ConditionPrefabMap[] goalPrefabs;
    private Dictionary<string, GameObject> goalDict;

    [SerializeField]
    private GameObject goalsMenu, leftGoal, middleGoal, rightGoal;

    public void moveObject(int old_x, int old_y, int new_x, int new_y) {
        var oldKind = kindMatrix[old_x, old_y];
        var oldObj = objMatrix[old_x, old_y];

        kindMatrix[old_x, old_y] = PuzzleObject.NTH;
        kindMatrix[new_x, new_y] = oldKind;

        objMatrix[old_x, old_y] = null;
        objMatrix[new_x, new_y] = oldObj;
    }

    public GameObject instantiateObject(GameObject obj, int i, int j) {
        float new_i = (i - colCount / 2f);
        float new_j = (j - rowCount / 2f);
        if (colCount % 2 == 0) {
            new_i += .5f;
        }
        if (rowCount % 2 == 0) {
            new_j += .5f;
        }
        objMatrix[i, j] = Instantiate(
            obj,
            new Vector3(new_i * cellDistance,
                new_j * cellDistance,
                0),
            Quaternion.identity
        );
        objMatrix[i, j].transform.parent = puzzleObjects;
        return objMatrix[i, j];
    }

    public void instantiateObjectFromKind(PuzzleManager.PuzzleObject obj, int i, int j) {
        if (prefabDict.ContainsKey(obj)) {
            instantiateObject(prefabDict[obj], i, j);
        }
    }

    public void instantiatePuzzleMatrix() {

        foreach (Transform t in puzzleObjects.GetComponentsInChildren<Transform>()) {
            if (t != puzzleObjects) {
                Destroy(t.gameObject);
            }
        }

        { // Creating the level's matrix
            catPosition = new int[] { stageInfo.catPositionX, stageInfo.catPositionY };
            dogPosition = new int[] { stageInfo.dogPositionX, stageInfo.dogPositionY };
            kindMatrix = new PuzzleObject[colCount, rowCount];
            for (int i = 0; i < colCount; i++) {
                for (int j = 0; j < rowCount; j++) {
                    kindMatrix[i, j] = stageInfo.matrix[i].entries[j];
                }
            }
        }

        { // Instantiating the level

            objMatrix = new GameObject[colCount, rowCount];

            for (var i = 0; i < colCount; i++) {
                for (var j = 0; j < rowCount; j++) {
                    if (prefabDict.ContainsKey(kindMatrix[i, j])) {
                        instantiateObject(prefabDict[kindMatrix[i, j]], i, j);
                    }
                }
            }

            if (stageInfo.hasCat)
                // Instantiates the cat
                catReference = instantiateObject(
                    prefabDict[PuzzleObject.CAT],
                    catPosition[0],
                    catPosition[1]
                );
            if (stageInfo.hasDog)
                // Instantiates the dog
                dogReference = instantiateObject(
                    prefabDict[PuzzleObject.DOG],
                    dogPosition[0],
                    dogPosition[1]
                );
        }
    }

    private void ShowGoals() {
        var goals = new List<string>();
        bool hasAnimalGoal = false;
        for (int i = 0; i < stageInfo.colCount; i++) {
            for (int j = 0; j < stageInfo.rowCount; j++) {
                if (kindMatrix[i, j] == PuzzleManager.PuzzleObject.GOAL_CAT && !goals.Contains("Cat reach goal")) {
                    goals.Add("Cat reach goal");
                }
                if (kindMatrix[i, j] == PuzzleManager.PuzzleObject.GOAL_DOG && !goals.Contains("Dog reach goal")) {
                    goals.Add("Dog reach goal");
                }
                if (kindMatrix[i, j] == PuzzleManager.PuzzleObject.EGG && !goals.Contains("Egg reach goal")) {
                    goals.Add("Egg reach goal");
                }
                if (kindMatrix[i, j] == PuzzleManager.PuzzleObject.ANIMAL_POINT) {
                    hasAnimalGoal = true;
                }
            }
        }
        for (int i = 0; i < stageInfo.colCount; i++) {
            for (int j = 0; j < stageInfo.rowCount; j++) {
                if (kindMatrix[i, j] == PuzzleManager.PuzzleObject.SQUIRREL) {
                    if (hasAnimalGoal) {
                        if (!goals.Contains("Squirrel reach goal"))
                            goals.Add("Squirrel reach goal");
                    } else {
                        if (!goals.Contains("Squirrel reach tree"))
                            goals.Add("Squirrel reach tree");
                    }
                }
                if (kindMatrix[i, j] == PuzzleManager.PuzzleObject.BIRD ||
                    kindMatrix[i, j] == PuzzleManager.PuzzleObject.TREE_WITH_BIRD ||
                    kindMatrix[i, j] == PuzzleManager.PuzzleObject.TREE_WITH_BIRD_X2 ||
                    kindMatrix[i, j] == PuzzleManager.PuzzleObject.TREE_WITH_BIRD_X3) {
                    if (hasAnimalGoal) {
                        if (!goals.Contains("Bird reach goal"))
                            goals.Add("Bird reach goal");
                    } else {
                        if (!goals.Contains("Bird reach tree"))
                            goals.Add("Bird reach tree");
                    }
                }
            }
        }
        if (goals.Count == 1) {
            Instantiate(goalDict[goals[0]], middleGoal.transform);
        } else if (goals.Count == 2) {
            goals.Sort();
            Instantiate(goalDict[goals[0]], leftGoal.transform);
            Instantiate(goalDict[goals[1]], rightGoal.transform);
        }
    }

    public void CloseGoalsMenu() {
        goalsMenu.SetActive(false);
    }

    private void Awake() {
        { // Creating the dictionaries of prefabs
            prefabDict = new Dictionary<PuzzleObject, GameObject>();
            foreach (var pair in prefabList) {
                prefabDict[pair.puzzleObject] = pair.prefab;
            }
            goalDict = new Dictionary<string, GameObject>();
            foreach (var pair in goalPrefabs) {
                goalDict[pair.condition] = pair.prefab;
            }
        }

        if (stageInfo == null) {
            Utils.loadPuzzle("PuzzleStage1-1");
        }

        // Destroys UI elements unused by this puzzle
        if (catList && !stageInfo.hasCat) {
            Destroy(catList);
        }
        if (dogList && !stageInfo.hasDog) {
            Destroy(dogList);
        }

        if (moveButton && !stageInfo.hasMove) {
            Destroy(moveButton);
        }
        if (grabButton && !stageInfo.hasGrab) {
            Destroy(grabButton);
        }
        if (waitButton && !stageInfo.hasWait) {
            Destroy(waitButton);
        }

        rowCount = stageInfo.rowCount;
        colCount = stageInfo.colCount;

        instantiatePuzzleMatrix();

        ShowGoals();
    }
}
