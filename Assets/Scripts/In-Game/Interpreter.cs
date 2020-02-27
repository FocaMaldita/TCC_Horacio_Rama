
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void moveCatUp() {
        if (Utils.IsCatInUpperCorner(puzzleManager)) {
            Debug.Log("Can't move cat");
            // Can't move
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.catPosition[0], puzzleManager.catPosition[1] + 1];
            var canMove = Utils.CanCatMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move cat up");
                puzzleManager.catPosition[1]++;
                StartCoroutine(moveCat('U'));
            } else if (canMove == 'P') {
                // Move cat up and push
            } else {
                // Cat is blocked
                Debug.Log("Cat is blocked");
            }
        }
    }

    void moveCatLeft() {
        if (Utils.IsCatInLeftCorner(puzzleManager)) {
            Debug.Log("Can't move cat");
            // Can't move
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.catPosition[0] - 1, puzzleManager.catPosition[1]];
            var canMove = Utils.CanCatMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move cat left");
                puzzleManager.catPosition[0]--;
                StartCoroutine(moveCat('L'));
            } else if (canMove == 'P') {
                // Move cat left and push
            } else {
                // Cat is blocked
                Debug.Log("Cat is blocked");
            }
        }
    }

    void moveCatRight() {
        if (Utils.IsCatInRightCorner(puzzleManager)) {
            Debug.Log("Can't move cat");
            // Can't move
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.catPosition[0] + 1, puzzleManager.catPosition[1]];
            var canMove = Utils.CanCatMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move cat right");
                puzzleManager.catPosition[0]++;
                StartCoroutine(moveCat('R'));
            } else if (canMove == 'P') {
                // Move cat right and push
            } else {
                // Cat is blocked
                Debug.Log("Cat is blocked");
            }
        }
    }

    void moveCatDown() {
        if (Utils.IsCatInBottomCorner(puzzleManager)) {
            Debug.Log("Can't move cat");
            // Can't move
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.catPosition[0], puzzleManager.catPosition[1] - 1];
            var canMove = Utils.CanCatMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move cat down");
                puzzleManager.catPosition[1]--;
                StartCoroutine(moveCat('D'));
            } else if (canMove == 'P') {
                // Move cat down and push
            } else {
                // Cat is blocked
                Debug.Log("Cat is blocked");
            }
        }
    }

    void moveDogUp() {
        if (Utils.IsDogInUpperCorner(puzzleManager)) {
            Debug.Log("Can't move dog");
            // Can't move dog
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.dogPosition[0], puzzleManager.dogPosition[1] + 1];
            var canMove = Utils.CanDogMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move dog up");
                puzzleManager.dogPosition[1]++;
                StartCoroutine(moveDog('U'));
            } else if (canMove == 'P') {
                // Move dog up and push
            } else {
                // Dog is blocked
                Debug.Log("Dog is blocked");
            }
        }
    }

    void moveDogLeft() {
        if (Utils.IsDogInLeftCorner(puzzleManager)) {
            Debug.Log("Can't move dog");
            // Can't move dog
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.dogPosition[0] - 1, puzzleManager.dogPosition[1]];
            var canMove = Utils.CanDogMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move dog left");
                puzzleManager.dogPosition[0]--;
                StartCoroutine(moveDog('L'));
            } else if (canMove == 'P') {
                // Move dog left and push
            } else {
                // Dog is blocked
                Debug.Log("Dog is blocked");
            }
        }
    }

    void moveDogRight() {
        if (Utils.IsDogInRightCorner(puzzleManager)) {
            Debug.Log("Can't move dog");
            // Can't move dog
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.dogPosition[0] + 1, puzzleManager.dogPosition[1]];
            var canMove = Utils.CanDogMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move dog right");
                puzzleManager.dogPosition[0]++;
                StartCoroutine(moveDog('R'));
            } else if (canMove == 'P') {
                // Move dog right and push
            } else {
                // Dog is blocked
                Debug.Log("Dog is blocked");
            }
        }
    }

    void moveDogDown() {
        if (Utils.IsDogInBottomCorner(puzzleManager)) {
            Debug.Log("Can't move dog");
            // Can't move dog
        } else {
            var obstacle = puzzleManager.kindMatrix[puzzleManager.dogPosition[0], puzzleManager.dogPosition[1] - 1];
            var canMove = Utils.CanDogMove(obstacle);
            if (canMove == 'Y') {
                Debug.Log("Move dog down");
                puzzleManager.dogPosition[1]--;
                StartCoroutine(moveDog('D'));
            } else if (canMove == 'P') {
                // Move dog down and push
            } else {
                // Dog is blocked
                Debug.Log("Dog is blocked");
            }
        }
    }

    void grabCat(char direction) {

        int pos_x, pos_y;
        if (direction == 'U') {
            pos_x = puzzleManager.catPosition[0];
            pos_y = puzzleManager.catPosition[1] + 1;
        } else if (direction == 'L') {
            pos_x = puzzleManager.catPosition[0] - 1;
            pos_y = puzzleManager.catPosition[1];
        } else if (direction == 'R') {
            pos_x = puzzleManager.catPosition[0] + 1;
            pos_y = puzzleManager.catPosition[1];
        } else if (direction == 'D') {
            pos_x = puzzleManager.catPosition[0];
            pos_y = puzzleManager.catPosition[1] - 1;
        } else {
            return;
        }

        if (Utils.IsCatInUpperCorner(puzzleManager)) {
            Debug.Log("Cat can't grab");
            // Cat can't grab
        } else {
            Debug.Log(catIsHolding);
            if (catIsHolding == null) {
                var obstacle = puzzleManager.kindMatrix[pos_x, pos_y];
                var obstacleObj = puzzleManager.objMatrix[pos_x, pos_y];
                var canGrab = Utils.CanCatGrab(obstacle);
                if (canGrab == 'Y') {
                    Debug.Log("Cat grab up");
                    catIsHolding = Instantiate(obstacleObj, new Vector3(100, 100, 100), Quaternion.identity);
                    catIsHoldingKind = obstacle;
                    puzzleManager.kindMatrix[pos_x, pos_y] = PuzzleManager.PuzzleObject.NTH;
                    Destroy(obstacleObj);
                } else {
                    // Cat can't grab this
                    Debug.Log("Cat can't grab this");
                }
            } else {
                if (Utils.CanPlaceObject(puzzleManager.kindMatrix[pos_x, pos_y]) == 'Y') {
                    puzzleManager.kindMatrix[pos_x, pos_y] = catIsHoldingKind;
                    puzzleManager.instantiateObject(catIsHolding, pos_x, pos_y);
                    Destroy(catIsHolding);
                    catIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
                }
            }
        }
    }

    IEnumerator moveCat(char direction) {
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

    IEnumerator moveDog(char direction) {
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
                                moveCatUp();
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDogUp();
                            }
                            break;

                        case Utils.InstructionType.MOVE_L:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                moveCatLeft();
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDogLeft();
                            }
                            break;

                        case Utils.InstructionType.MOVE_R:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                moveCatRight();
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDogRight();
                            }
                            break;

                        case Utils.InstructionType.MOVE_D:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                moveCatDown();
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                moveDogDown();
                            }
                            break;

                        case Utils.InstructionType.GRAB_U:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('U');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                //grabDog('U');
                            }
                            break;

                        case Utils.InstructionType.GRAB_L:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('L');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                //grabDog('L');
                            }
                            break;

                        case Utils.InstructionType.GRAB_R:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('R');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                //grabDog('R');
                            }
                            break;

                        case Utils.InstructionType.GRAB_D:
                            if (character.owner == PuzzleManager.PuzzleObject.CAT) {
                                grabCat('D');
                            } else if (character.owner == PuzzleManager.PuzzleObject.DOG) {
                                //grabDog('D');
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
