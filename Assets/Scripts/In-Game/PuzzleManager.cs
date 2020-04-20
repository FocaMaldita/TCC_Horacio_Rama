
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
        GOAL=51,
        ANIMAL_POINT=52,
        ITEM_POINT=53,
        ANIMAL_POINT_SQUIRREL=61,
        ANIMAL_POINT_BIRD=62,
        ANIMAL_POINT_BIRD_X2=63,
        ANIMAL_POINT_BIRD_X3=64,
        ITEM_POINT_EGG=71,
        ITEM_POINT_EGG_X2=72,
        ITEM_POINT_EGG_X3=73,
    }

    [System.Serializable]
    public struct PuzzleObjectPrefabMap{
        public PuzzleObject puzzleObject;
        public GameObject prefab;
    }
    
    public PuzzleObjectPrefabMap[] prefabList;
    private Dictionary<PuzzleObject, GameObject> prefabDict;

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
        return objMatrix[i, j];
    }

    public GameObject instantiateObjectFromKind(PuzzleManager.PuzzleObject obj, int i, int j) {
        return instantiateObject(prefabDict[obj], i, j);
    }

    void Awake() {
        { // Creating the dictionary of prefabs
            prefabDict = new Dictionary<PuzzleObject, GameObject>();
            foreach (var pair in prefabList) {
                prefabDict[pair.puzzleObject] = pair.prefab;
            }
        }

        if (stageInfo == null) {
            Utils.loadPuzzle("PuzzleStage1-1");
        }

        // Destroys UI elements unused by this puzzle
        if (!stageInfo.hasCat) {
            Destroy(catList);
        }
        if (!stageInfo.hasDog) {
            Destroy(dogList);
        }

        if (!stageInfo.hasMove) {
            Destroy(moveButton);
        }
        if (!stageInfo.hasGrab) {
            Destroy(grabButton);
        }
        if (!stageInfo.hasWait) {
            Destroy(waitButton);
        }

        rowCount = stageInfo.rowCount;
        colCount = stageInfo.colCount;

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
}
