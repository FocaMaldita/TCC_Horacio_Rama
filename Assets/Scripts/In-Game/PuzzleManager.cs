
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
        ROCK=43,
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

    public void instantiateObject(GameObject obj, int i, int j) {
        objMatrix[i, j] = Instantiate(
            obj,
            new Vector3((i - colCount / 2) * cellDistance,
                (j - rowCount / 2) * cellDistance,
                0),
            Quaternion.identity
        );
    }

    void Awake() {
        { // Creating the dictionary of prefabs
            prefabDict = new Dictionary<PuzzleObject, GameObject>();
            foreach (var pair in prefabList) {
                prefabDict[pair.puzzleObject] = pair.prefab;
            }
        }

        if (stageInfo == null) {
            Utils.loadPuzzle("PuzzleStage1");
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
            if (stageInfo.hasCat)
                // Instantiates the cat
                catReference = Instantiate(
                    prefabDict[PuzzleObject.CAT],
                    new Vector3((catPosition[0] - colCount / 2) * cellDistance,
                        (catPosition[1] - rowCount / 2) * cellDistance,
                        0),
                    Quaternion.identity
                );
            if (stageInfo.hasDog)
                // Instantiates the dog
                dogReference = Instantiate(
                    prefabDict[PuzzleObject.DOG],
                    new Vector3((dogPosition[0] - colCount / 2) * cellDistance,
                        (dogPosition[1] - rowCount / 2) * cellDistance,
                        0),
                    Quaternion.identity
                );

            objMatrix = new GameObject[colCount, rowCount];

            for (var i = 0; i < colCount; i++) {
                for (var j = 0; j < rowCount; j++) {
                    if (prefabDict.ContainsKey(kindMatrix[i, j])) {
                        instantiateObject(prefabDict[kindMatrix[i, j]], i, j);
                    }
                }
            }
        }
    }
}
