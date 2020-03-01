
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour {

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

    public void instantiateObject(GameObject obj, int i, int j) {
        objMatrix[i, j] = Instantiate(
            obj,
            new Vector3((i - rowCount / 2) * cellDistance,
                (j - colCount / 2) * cellDistance,
                0),
            Quaternion.identity
        );
    }

    void Start() {
        { // Creating the dictionary of prefabs
            prefabDict = new Dictionary<PuzzleObject, GameObject>();
            foreach (var pair in prefabList) {
                prefabDict[pair.puzzleObject] = pair.prefab;
            }
        }
        { // Creating the level's matrix
            kindMatrix = new PuzzleObject[9, 9];
            catPosition = new int[] { 4, 4 };
            dogPosition = new int[] { 4, 3 };
            kindMatrix[2, 4] = PuzzleObject.ROCK;
            kindMatrix[2, 3] = PuzzleObject.ROCK;
            kindMatrix[2, 5] = PuzzleObject.ROCK;
            kindMatrix[0, 3] = PuzzleObject.BIRD;
            kindMatrix[5, 6] = PuzzleObject.BIRD;
        }

        rowCount = kindMatrix.GetLength(0);
        colCount = kindMatrix.GetLength(1);

        { // Instantiating the level
            if (prefabDict.ContainsKey(PuzzleObject.CAT))
                // Instantiates the cat
                catReference = Instantiate(
                    prefabDict[PuzzleObject.CAT],
                    new Vector3((catPosition[0] - rowCount / 2) * cellDistance,
                        (catPosition[1] - colCount / 2) * cellDistance,
                        0),
                    Quaternion.identity
                );
            if (prefabDict.ContainsKey(PuzzleObject.DOG))
                // Instantiates the dog
                dogReference = Instantiate(
                    prefabDict[PuzzleObject.DOG],
                    new Vector3((dogPosition[0] - rowCount / 2) * cellDistance,
                        (dogPosition[1] - colCount / 2) * cellDistance,
                        0),
                    Quaternion.identity
                );

            objMatrix = new GameObject[rowCount, colCount];

            for (var i=0; i < rowCount; i++) {
                for (var j=0; j < colCount; j++) {
                    if (prefabDict.ContainsKey(kindMatrix[i, j])) {
                        instantiateObject(prefabDict[kindMatrix[i, j]], i, j);
                    }
                }
            }
        }
    }
}
