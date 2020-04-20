
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interpreter : MonoBehaviour {

    public float secondsPerMove = .1f;

    public float secondsBetweenMoves = .5f;

    [HideInInspector]
    public bool isInterpreting = false;

    [SerializeField]
    private PuzzleManager puzzleManager;

    [System.Serializable]
    public struct InstructionListOwner {
        public PuzzleManager.PuzzleObject owner;
        public InstructionList instList;
    }

    [SerializeField]
    private InstructionListOwner[] instructionLists;

    private GameObject catIsHolding = null;
    private PuzzleManager.PuzzleObject catIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
    private GameObject dogIsHolding = null;
    private PuzzleManager.PuzzleObject dogIsHoldingKind = PuzzleManager.PuzzleObject.NTH;

    [SerializeField]
    private Image catHolding, dogHolding;

    int[] getCatDestination(char direction) {
        int[] ret = new int[2] { 0, 0 };

        if (direction == 'U') {
            ret[0] = puzzleManager.catPosition[0];
            ret[1] = puzzleManager.catPosition[1] + 1;
        } else if (direction == 'L') {
            ret[0] = puzzleManager.catPosition[0] - 1;
            ret[1] = puzzleManager.catPosition[1];
        } else if (direction == 'R') {
            ret[0] = puzzleManager.catPosition[0] + 1;
            ret[1] = puzzleManager.catPosition[1];
        } else if (direction == 'D') {
            ret[0] = puzzleManager.catPosition[0];
            ret[1] = puzzleManager.catPosition[1] - 1;
        } else {
            Debug.Log("getCatDirection received invalid parameter: " + direction);
        }
        return ret;
    }

    int[] getDogDestination(char direction) {
        int[] ret = new int[2] { 0, 0 };

        if (direction == 'U') {
            ret[0] = puzzleManager.dogPosition[0];
            ret[1] = puzzleManager.dogPosition[1] + 1;
        } else if (direction == 'L') {
            ret[0] = puzzleManager.dogPosition[0] - 1;
            ret[1] = puzzleManager.dogPosition[1];
        } else if (direction == 'R') {
            ret[0] = puzzleManager.dogPosition[0] + 1;
            ret[1] = puzzleManager.dogPosition[1];
        } else if (direction == 'D') {
            ret[0] = puzzleManager.dogPosition[0];
            ret[1] = puzzleManager.dogPosition[1] - 1;
        } else {
            Debug.Log("getDogDirection received invalid parameter: " + direction);
        }
        return ret;
    }

    void moveCat(char direction) {

        int[] pos = getCatDestination(direction);

        if (Utils.IsCatInCorner(puzzleManager, direction)) {
            Debug.Log("Can't move cat");
            // Can't move cat
        } else {
            var obstacle = puzzleManager.kindMatrix[pos[0], pos[1]];
            var canMove = Utils.CanCatMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move cat " + direction);
                switch (direction) {
                    case 'U':
                        puzzleManager.catPosition[1]++;
                        break;
                    case 'L':
                        puzzleManager.catPosition[0]--;
                        break;
                    case 'R':
                        puzzleManager.catPosition[0]++;
                        break;
                    case 'D':
                        puzzleManager.catPosition[1]--;
                        break;
                }
                StartCoroutine(moveCatTransition(direction));
            } else if (canMove == 'P') {
                // Move cat up and push
            } else {
                // Cat is blocked
                Debug.Log("Cat is blocked");
            }
        }
    }

    void moveDog(char direction) {

        int[] pos = getDogDestination(direction);

        if (Utils.IsDogInCorner(puzzleManager, direction)) {
            Debug.Log("Can't move dog");
            // Can't move dog
        } else {
            var obstacle = puzzleManager.kindMatrix[pos[0], pos[1]];
            var canMove = Utils.CanDogMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move dog " + direction);
                switch (direction) {
                    case 'U':
                        puzzleManager.dogPosition[1]++;
                        break;
                    case 'L':
                        puzzleManager.dogPosition[0]--;
                        break;
                    case 'R':
                        puzzleManager.dogPosition[0]++;
                        break;
                    case 'D':
                        puzzleManager.dogPosition[1]--;
                        break;
                }

                StartCoroutine(moveDogTransition(direction));
            } else if (canMove == 'P') {
                // Move dog up and push

                if (Utils.IsThingPushedToCorner(puzzleManager, direction, pos[0], pos[1])) {
                    // Dog is blocked
                    Debug.Log("Dog is blocked");
                } else {
                    // Dog moves and pushes thing
                    var obstacleObj = puzzleManager.objMatrix[pos[0], pos[1]];
                    switch (direction) {
                        case 'U':
                            puzzleManager.dogPosition[1]++;
                            puzzleManager.moveObject(pos[0], pos[1], pos[0], pos[1] + 1);
                            break;
                        case 'L':
                            puzzleManager.dogPosition[0]--;
                            puzzleManager.moveObject(pos[0], pos[1], pos[0] - 1, pos[1]);
                            break;
                        case 'R':
                            puzzleManager.dogPosition[0]++;
                            puzzleManager.moveObject(pos[0], pos[1], pos[0] + 1, pos[1]);
                            break;
                        case 'D':
                            puzzleManager.dogPosition[1]--;
                            puzzleManager.moveObject(pos[0], pos[1], pos[0], pos[1] - 1);
                            break;
                    }

                    StartCoroutine(moveThingTransition(obstacleObj.transform, direction));
                    StartCoroutine(moveDogTransition(direction));
                }

            } else {
                // Dog is blocked
                Debug.Log("Dog is blocked");
            }
        }
    }

    void grabCat(char direction) {
        
        int[] pos = getCatDestination(direction);

        if (Utils.IsCatInCorner(puzzleManager, direction)) {
            Debug.Log("Cat can't grab");
            // Cat can't grab
        } else {
            if (catIsHolding == null) {
                var obstacle = puzzleManager.kindMatrix[pos[0], pos[1]];
                var obstacleObj = puzzleManager.objMatrix[pos[0], pos[1]];
                var canGrab = Utils.CanCatGrab(obstacle);
                if (canGrab == 'Y') {
                    Debug.Log("Cat grab up");
                    catIsHolding = Instantiate(obstacleObj, new Vector3(100, 100, 100), Quaternion.identity);
                    catIsHoldingKind = obstacle;
                    puzzleManager.kindMatrix[pos[0], pos[1]] = PuzzleManager.PuzzleObject.NTH;
                    var obstacleSprite = obstacleObj.GetComponent<SpriteRenderer>();
                    catHolding.color = new Color(obstacleSprite.color.r, obstacleSprite.color.g, obstacleSprite.color.b, .5f);
                    catHolding.sprite = obstacleSprite.sprite;
                    Destroy(obstacleObj);
                } else {
                    // Cat can't grab this
                    Debug.Log("Cat can't grab this");
                }
            } else {
                var canPlace = Utils.CanPlaceObject(catIsHoldingKind, puzzleManager.kindMatrix[pos[0], pos[1]]);
                if (canPlace != PuzzleManager.PuzzleObject.NTH) {
                    Destroy(puzzleManager.objMatrix[pos[0], pos[1]]);
                    puzzleManager.kindMatrix[pos[0], pos[1]] = canPlace;
                    puzzleManager.instantiateObjectFromKind(canPlace, pos[0], pos[1]);
                    Destroy(catIsHolding);
                    catIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
                    catHolding.color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    void grabDog(char direction) {

        int[] pos = getDogDestination(direction);

        if (Utils.IsDogInCorner(puzzleManager, direction)) {
            Debug.Log("Dog can't grab");
            // Dog can't grab
        } else {
            if (dogIsHolding == null) {
                var obstacle = puzzleManager.kindMatrix[pos[0], pos[1]];
                var obstacleObj = puzzleManager.objMatrix[pos[0], pos[1]];
                var canGrab = Utils.CanDogGrab(obstacle);
                if (canGrab == 'Y') {
                    Debug.Log("Dog grab up");
                    dogIsHolding = Instantiate(obstacleObj, new Vector3(100, 100, 100), Quaternion.identity);
                    dogIsHoldingKind = obstacle;
                    puzzleManager.kindMatrix[pos[0], pos[1]] = PuzzleManager.PuzzleObject.NTH;
                    var obstacleSprite = obstacleObj.GetComponent<SpriteRenderer>();
                    dogHolding.color = new Color(obstacleSprite.color.r, obstacleSprite.color.g, obstacleSprite.color.b, .5f);
                    dogHolding.sprite = obstacleSprite.sprite;
                    Destroy(obstacleObj);
                } else {
                    // Dog can't grab this
                    Debug.Log("Dog can't grab this");
                }
            } else {
                var canPlace = Utils.CanPlaceObject(dogIsHoldingKind, puzzleManager.kindMatrix[pos[0], pos[1]]);
                if (canPlace != PuzzleManager.PuzzleObject.NTH) {
                    Destroy(puzzleManager.objMatrix[pos[0], pos[1]]);
                    puzzleManager.kindMatrix[pos[0], pos[1]] = canPlace;
                    puzzleManager.instantiateObjectFromKind(canPlace, pos[0], pos[1]);
                    Destroy(dogIsHolding);
                    dogIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
                    dogHolding.color = new Color(1, 1, 1, 0);
                }
            }
        }
    }

    IEnumerator moveCatTransition(char direction) {
        var oldPos = puzzleManager.catReference.transform.position;
        switch (direction) {
            case 'U':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.catReference.transform.position = new Vector3(
                        oldPos.x,
                        oldPos.y + (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10f);
                }
                break;
            case 'L':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.catReference.transform.position = new Vector3(
                        oldPos.x - (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.y,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;
            case 'R':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.catReference.transform.position = new Vector3(
                        oldPos.x + (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.y,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;
            case 'D':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.catReference.transform.position = new Vector3(
                        oldPos.x,
                        oldPos.y - (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;

        }
        yield break;
    }

    IEnumerator moveDogTransition(char direction) {
        var oldPos = puzzleManager.dogReference.transform.position;
        switch (direction) {
            case 'U':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.dogReference.transform.position = new Vector3(
                        oldPos.x,
                        oldPos.y + (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10f);
                }
                break;
            case 'L':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.dogReference.transform.position = new Vector3(
                        oldPos.x - (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.y,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;
            case 'R':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.dogReference.transform.position = new Vector3(
                        oldPos.x + (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.y,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;
            case 'D':
                for (var i = 0; i < 10; i++) {
                    puzzleManager.dogReference.transform.position = new Vector3(
                        oldPos.x,
                        oldPos.y - (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;

        }
        yield break;
    }

    IEnumerator moveThingTransition(Transform thing, char direction) {
        var oldPos = thing.position;
        switch (direction) {
            case 'U':
                for (var i = 0; i < 10; i++) {
                    thing.position = new Vector3(
                        oldPos.x,
                        oldPos.y + (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10f);
                }
                break;
            case 'L':
                for (var i = 0; i < 10; i++) {
                    thing.position = new Vector3(
                        oldPos.x - (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.y,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;
            case 'R':
                for (var i = 0; i < 10; i++) {
                    thing.position = new Vector3(
                        oldPos.x + (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.y,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;
            case 'D':
                for (var i = 0; i < 10; i++) {
                    thing.position = new Vector3(
                        oldPos.x,
                        oldPos.y - (i + 1) / 10f * puzzleManager.cellDistance,
                        oldPos.z
                    );
                    yield return new WaitForSeconds(secondsPerMove / 10);
                }
                break;

        }
        yield break;
    }

    IEnumerator interpretationEvent() {
        var largestList = 0;
        foreach (var character in instructionLists) {
            if (character.instList.list.Count > largestList) {
                largestList = character.instList.list.Count;
            }
        }

        for (var i=0; i < largestList; i++) {
            foreach (var character in instructionLists) {
                if (i < character.instList.list.Count) {
                    var action = character.instList.list[i].GetComponent<InstructionListNode>().instructionType;
                    switch (action) {

                        case Utils.InstructionType.MOVE_U:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                moveCat('U');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDog('U');
                            }
                            break;

                        case Utils.InstructionType.MOVE_L:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                moveCat('L');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDog('L');
                            }
                            break;

                        case Utils.InstructionType.MOVE_R:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                moveCat('R');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDog('R');
                            }
                            break;

                        case Utils.InstructionType.MOVE_D:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                moveCat('D');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDog('D');
                            }
                            break;

                        case Utils.InstructionType.GRAB_U:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('U');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                grabDog('U');
                            }
                            break;

                        case Utils.InstructionType.GRAB_L:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('L');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                grabDog('L');
                            }
                            break;

                        case Utils.InstructionType.GRAB_R:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('R');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                grabDog('R');
                            }
                            break;

                        case Utils.InstructionType.GRAB_D:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('D');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                grabDog('D');
                            }
                            break;
                    }
                }
            }
            yield return new WaitForSeconds(secondsPerMove + secondsBetweenMoves);
        }
        yield break;
    }

    public void interpret() {
        isInterpreting = true;

        StartCoroutine(interpretationEvent());

        isInterpreting = false;
    }
}
