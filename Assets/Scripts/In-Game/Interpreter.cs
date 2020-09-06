
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interpreter : MonoBehaviour {

    public float secondsPerMove = .1f;

    public float secondsBetweenMoves = .5f;

    [HideInInspector]
    public static bool isInterpreting = false;

    [SerializeField]
    private PuzzleManager puzzleManager;

    [SerializeField]
    private InstructionList catInstructionList, dogInstructionList;

    [SerializeField]
    private CurrentInstructionArrow catArrow, dogArrow;

    private GameObject catIsHolding = null;
    private PuzzleManager.PuzzleObject catIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
    private GameObject dogIsHolding = null;
    private PuzzleManager.PuzzleObject dogIsHoldingKind = PuzzleManager.PuzzleObject.NTH;

    [SerializeField]
    private Image catHolding, dogHolding;

    [SerializeField]
    private GameObject endMenu;

    private bool missionFailed = false;
    private MissionResult.Condition missionResult = MissionResult.Condition.SUCCESS;

    private bool catIsWaiting = false, dogIsWaiting = false;
    private int catIndex = 0, dogIndex = 0;

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
            missionFailed = true;
            missionResult = MissionResult.Condition.CAT_WALK_FAIL;
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
                missionFailed = true;
                missionResult = MissionResult.Condition.CAT_WALK_FAIL;
            }
        }
    }

    void moveDog(char direction) {

        int[] pos = getDogDestination(direction);

        if (Utils.IsDogInCorner(puzzleManager, direction)) {
            Debug.Log("Can't move dog");
            missionFailed = true;
            missionResult = MissionResult.Condition.DOG_WALK_FAIL;
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
                    missionFailed = true;
                    missionResult = MissionResult.Condition.DOG_WALK_FAIL;
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
                missionFailed = true;
                missionResult = MissionResult.Condition.DOG_WALK_FAIL;
            }
        }
    }

    bool willCollide(Utils.InstructionType catAction, Utils.InstructionType dogAction) {

        if (!PuzzleManager.stageInfo.hasCat || !PuzzleManager.stageInfo.hasDog) {
            return false;
        }

        int cat_x = puzzleManager.catPosition[0];
        int cat_y = puzzleManager.catPosition[1];
        int dog_x = puzzleManager.dogPosition[0];
        int dog_y = puzzleManager.dogPosition[1];

        int[] cat_dest = { 0, 0 }, dog_dest = { 0, 0 };
        switch (catAction) {
            case Utils.InstructionType.MOVE_U:
                cat_dest = getCatDestination('U');
                break;

            case Utils.InstructionType.MOVE_L:
                cat_dest = getCatDestination('L');
                break;

            case Utils.InstructionType.MOVE_R:
                cat_dest = getCatDestination('R');
                break;

            case Utils.InstructionType.MOVE_D:
                cat_dest = getCatDestination('D');
                break;

            case Utils.InstructionType.GRAB_U:
                cat_dest = getCatDestination('U');
                break;

            case Utils.InstructionType.GRAB_L:
                cat_dest = getCatDestination('L');
                break;

            case Utils.InstructionType.GRAB_R:
                cat_dest = getCatDestination('R');
                break;

            case Utils.InstructionType.GRAB_D:
                cat_dest = getCatDestination('D');
                break;
            default:
                cat_dest = new int[] { -1, -1 };
                break;
        }
        switch (dogAction) {
            case Utils.InstructionType.MOVE_U:
                dog_dest = getDogDestination('U');
                break;

            case Utils.InstructionType.MOVE_L:
                dog_dest = getDogDestination('L');
                break;

            case Utils.InstructionType.MOVE_R:
                dog_dest = getDogDestination('R');
                break;

            case Utils.InstructionType.MOVE_D:
                dog_dest = getDogDestination('D');
                break;

            case Utils.InstructionType.GRAB_U:
                dog_dest = getDogDestination('U');
                break;

            case Utils.InstructionType.GRAB_L:
                dog_dest = getDogDestination('L');
                break;

            case Utils.InstructionType.GRAB_R:
                dog_dest = getDogDestination('R');
                break;

            case Utils.InstructionType.GRAB_D:
                dog_dest = getDogDestination('D');
                break;
            default:
                dog_dest = new int[] { -2, -2 };
                break;
        }
        int cat_dest_x = cat_dest[0];
        int cat_dest_y = cat_dest[1];
        int dog_dest_x = dog_dest[0];
        int dog_dest_y = dog_dest[1];

        // Both moving
        if (Utils.IsMovementType(catAction) && Utils.IsMovementType(dogAction)) {
            if (cat_dest_x == dog_dest_x && cat_dest_y == dog_dest_y) {
                // They collided
                Debug.Log("They collided");
                missionResult = MissionResult.Condition.CAT_DOG_WALK_INTO_EACH_OTHER_FAIL;
                return true;
            }
            if (cat_dest_x == dog_x && cat_dest_y == dog_y && cat_x == dog_dest_x && cat_y == dog_dest_y) {
                // They went through each other
                Debug.Log("They went through each other");
                missionResult = MissionResult.Condition.CAT_DOG_WALK_INTO_EACH_OTHER_FAIL;
                return true;
            }
        }
        // Cat moving
        if (Utils.IsMovementType(catAction)) {
            if (cat_dest_x == dog_dest_x && cat_dest_y == dog_dest_y) {
                // Cat ran into grab
                Debug.Log("Cat ran into grab");
                if (dogIsHoldingKind == PuzzleManager.PuzzleObject.NTH) {
                    missionResult = MissionResult.Condition.DOG_GRAB_FAIL;
                } else {
                    missionResult = MissionResult.Condition.DOG_PLACE_FAIL;
                }
                return true;
            }
            if (cat_dest_x == dog_x && cat_dest_y == dog_y) {
                // Cat ran into dog
                Debug.Log("Cat ran into dog");
                missionResult = MissionResult.Condition.CAT_WALK_FAIL;
                return true;
            }
        }
        // Dog moving
        if (Utils.IsMovementType(dogAction)) {
            if (cat_dest_x == dog_dest_x && cat_dest_y == dog_dest_y) {
                // Dog ran into grab
                Debug.Log("Dog ran into grab");
                if (catIsHoldingKind == PuzzleManager.PuzzleObject.NTH) {
                    missionResult = MissionResult.Condition.CAT_GRAB_FAIL;
                } else {
                    missionResult = MissionResult.Condition.CAT_PLACE_FAIL;
                }
                return true;
            }
            if (dog_dest_x == cat_x && dog_dest_y == cat_y) {
                // Cat ran into dog
                Debug.Log("Dog ran into cat");
                missionResult = MissionResult.Condition.DOG_WALK_FAIL;
                return true;
            }
        }
        // Grab
        if (cat_dest_x == dog_x && cat_dest_y == dog_y) {
            // Cat grabbed dog
            Debug.Log("Cat grabbed dog");
            if (catIsHoldingKind == PuzzleManager.PuzzleObject.NTH) {
                missionResult = MissionResult.Condition.CAT_GRAB_FAIL;
            } else {
                missionResult = MissionResult.Condition.CAT_PLACE_FAIL;
            }
            return true;
        }
        if (cat_x == dog_dest_x && cat_y == dog_dest_y) {
            // Dog grabbed cat
            Debug.Log("Dog grabbed cat");
            if (dogIsHoldingKind == PuzzleManager.PuzzleObject.NTH) {
                missionResult = MissionResult.Condition.DOG_GRAB_FAIL;
            } else {
                missionResult = MissionResult.Condition.DOG_PLACE_FAIL;
            }
            return true;
        }
        if (cat_dest_x == dog_dest_x && cat_dest_y == dog_dest_y) {
            // Grabs clashed
            Debug.Log("Grabs clashed");
            if (catIsHoldingKind == PuzzleManager.PuzzleObject.NTH) {
                missionResult = MissionResult.Condition.CAT_GRAB_FAIL;
            } else {
                missionResult = MissionResult.Condition.CAT_PLACE_FAIL;
            }
            return true;
        }

        return false;
    }

    IEnumerator grabCat(char direction) {
        
        int[] pos = getCatDestination(direction);

        if (Utils.IsCatInCorner(puzzleManager, direction)) {
            Debug.Log("Cat can't grab");
            missionFailed = true;
            yield break;
            // Cat can't grab
        } else {
            var oldPos = puzzleManager.catReference.transform.position;
            switch (direction) {
                case 'U':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y + i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10f);
                    }
                    break;
                case 'L':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x - i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'R':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x + i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'D':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y - i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;

            }
            if (catIsHolding == null) {
                var obstacle = puzzleManager.kindMatrix[pos[0], pos[1]];
                var canGrab = Utils.CanCatGrab(obstacle);
                // Cannot grab treeUpper if not below it
                if (direction != 'U' && (new List<PuzzleManager.PuzzleObject> {
                    PuzzleManager.PuzzleObject.TREE_UPPER,
                    PuzzleManager.PuzzleObject.TREE_WITH_BIRD,
                    PuzzleManager.PuzzleObject.TREE_WITH_BIRD_X2,
                    PuzzleManager.PuzzleObject.TREE_WITH_BIRD_X3,
                }).Contains(obstacle)) {
                    canGrab = PuzzleManager.PuzzleObject.NTH;
                }
                if (canGrab != PuzzleManager.PuzzleObject.NTH) {
                    var obstacleObj = puzzleManager.prefabDict[canGrab];
                    var obstacleSprite = obstacleObj.GetComponent<SpriteRenderer>();
                    var whatRemains = Utils.WhatRemainsAfterRemovingObject(obstacle);
                    catIsHolding = Instantiate(obstacleObj, new Vector3(100, 100, 100), Quaternion.identity);
                    catIsHoldingKind = canGrab;
                    puzzleManager.kindMatrix[pos[0], pos[1]] = whatRemains;
                    catHolding.color = new Color(obstacleSprite.color.r, obstacleSprite.color.g, obstacleSprite.color.b, .5f);
                    catHolding.sprite = obstacleSprite.sprite;
                    Destroy(puzzleManager.objMatrix[pos[0], pos[1]]);
                    puzzleManager.instantiateObjectFromKind(whatRemains, pos[0], pos[1]);
                } else {
                    // Cat can't grab this
                    Debug.Log("Cat can't grab this");
                    missionFailed = true;
                    missionResult = MissionResult.Condition.CAT_GRAB_FAIL;
                }
            } else {
                var canPlace = Utils.CanPlaceObject(catIsHoldingKind, puzzleManager.kindMatrix[pos[0], pos[1]]);
                // Cannot grab treeUpper if not below it
                if (direction != 'U' && (new List<PuzzleManager.PuzzleObject> {
                    PuzzleManager.PuzzleObject.TREE_UPPER,
                    PuzzleManager.PuzzleObject.TREE_WITH_BIRD,
                    PuzzleManager.PuzzleObject.TREE_WITH_BIRD_X2,
                    PuzzleManager.PuzzleObject.TREE_WITH_BIRD_X3,
                }).Contains(canPlace)) {
                    canPlace = PuzzleManager.PuzzleObject.NTH;
                }
                if (canPlace != PuzzleManager.PuzzleObject.NTH) {
                    Destroy(puzzleManager.objMatrix[pos[0], pos[1]]);
                    puzzleManager.kindMatrix[pos[0], pos[1]] = canPlace;
                    puzzleManager.instantiateObjectFromKind(canPlace, pos[0], pos[1]);
                    Destroy(catIsHolding);
                    catIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
                    catHolding.color = new Color(1, 1, 1, 0);
                } else {
                    // Cat can't place here
                    Debug.Log("Cat can't place here");
                    missionFailed = true;
                    missionResult = MissionResult.Condition.CAT_PLACE_FAIL;
                }
            }
            switch (direction) {
                case 'U':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y + i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10f);
                    }
                    break;
                case 'L':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x - i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'R':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x + i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'D':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.catReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y - i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;

            }
        }
    }

    IEnumerator grabDog(char direction) {

        int[] pos = getDogDestination(direction);

        if (Utils.IsDogInCorner(puzzleManager, direction)) {
            Debug.Log("Dog can't grab");
            missionFailed = true;
            // Dog can't grab
        } else {
            var oldPos = puzzleManager.dogReference.transform.position;
            switch (direction) {
                case 'U':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y + i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10f);
                    }
                    break;
                case 'L':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x - i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'R':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x + i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'D':
                    for (var i = 0; i < 5; i++) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y - i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;

            }
            if (dogIsHolding == null) {
                var obstacle = puzzleManager.kindMatrix[pos[0], pos[1]];
                var canGrab = Utils.CanDogGrab(obstacle);
                if (canGrab != PuzzleManager.PuzzleObject.NTH) {
                    var obstacleObj = puzzleManager.prefabDict[canGrab];
                    var obstacleSprite = obstacleObj.GetComponent<SpriteRenderer>();
                    var whatRemains = Utils.WhatRemainsAfterRemovingObject(obstacle);
                    dogIsHolding = Instantiate(obstacleObj, new Vector3(100, 100, 100), Quaternion.identity);
                    dogIsHoldingKind = canGrab;
                    puzzleManager.kindMatrix[pos[0], pos[1]] = whatRemains;
                    dogHolding.color = new Color(obstacleSprite.color.r, obstacleSprite.color.g, obstacleSprite.color.b, .5f);
                    dogHolding.sprite = obstacleSprite.sprite;
                    Destroy(puzzleManager.objMatrix[pos[0], pos[1]]);
                    puzzleManager.instantiateObjectFromKind(whatRemains, pos[0], pos[1]);
                } else {
                    // Dog can't grab this
                    Debug.Log("Dog can't grab this");
                    missionFailed = true;
                    missionResult = MissionResult.Condition.DOG_GRAB_FAIL;
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
                } else {
                    // Dog can't place here
                    Debug.Log("Dog can't place here");
                    missionFailed = true;
                    missionResult = MissionResult.Condition.DOG_PLACE_FAIL;
                }
            }
            switch (direction) {
                case 'U':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y + i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10f);
                    }
                    break;
                case 'L':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x - i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'R':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x + i / 10f * puzzleManager.cellDistance,
                            oldPos.y,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;
                case 'D':
                    for (var i = 4; i >= 0; i--) {
                        puzzleManager.dogReference.transform.position = new Vector3(
                            oldPos.x,
                            oldPos.y - i / 10f * puzzleManager.cellDistance,
                            oldPos.z
                        );
                        yield return new WaitForSeconds(secondsPerMove / 10);
                    }
                    break;

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

    void waitCat() {
        if (dogIsWaiting) {
            catIsWaiting = false;
            dogIsWaiting = false;
            catNextStep();
            dogNextStep();
        } else {
            catIsWaiting = true;
        }
    }

    void waitDog() {
        if (catIsWaiting) {
            dogIsWaiting = false;
            catIsWaiting = false;
            dogNextStep();
            catNextStep();
        } else {
            dogIsWaiting = true;
        }
    }

    void catNextStep() {
        var catLoop = catInstructionList.transform.Find("Backdrop").Find("Node" + catIndex).gameObject.GetComponent<InstructionNodeLoop>();
        if (catLoop.loopSize > 0) {
            catIndex = catLoop.origin;
            catLoop.loopSize--;
        } else {
            catLoop.recoverSize();
            catIndex++;
        }
    }

    void dogNextStep() {
        var dogLoop = dogInstructionList.transform.Find("Backdrop").Find("Node" + dogIndex).gameObject.GetComponent<InstructionNodeLoop>();
        if (dogLoop.loopSize > 0) {
            dogIndex = dogLoop.origin;
            dogLoop.loopSize--;
        } else {
            dogLoop.recoverSize();
            dogIndex++;
        }
    }

    IEnumerator interpretationEvent() {
        catIndex = 0;
        dogIndex = 0;
        if (PuzzleManager.stageInfo.hasCat) {
            catArrow.gameObject.SetActive(true);
        }
        if (PuzzleManager.stageInfo.hasDog) {
            dogArrow.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(catArrow.duration + secondsBetweenMoves);
        while (catIndex < catInstructionList.list.Count || dogIndex < dogInstructionList.list.Count) {
            Utils.InstructionType catAction = Utils.InstructionType.WAIT, dogAction = Utils.InstructionType.WAIT;
            if (PuzzleManager.stageInfo.hasCat && catIndex < catInstructionList.list.Count) {
                catArrow.target = catIndex;
            }
            if (PuzzleManager.stageInfo.hasDog && dogIndex < dogInstructionList.list.Count) {
                dogArrow.target = dogIndex;
            }
            if (catIndex < catInstructionList.list.Count) {
                catAction = catInstructionList.list[catIndex].GetComponent<InstructionListNode>().instructionType;
            }
            if (dogIndex < dogInstructionList.list.Count) {
                dogAction = dogInstructionList.list[dogIndex].GetComponent<InstructionListNode>().instructionType;
            }
            if (willCollide(catAction, dogAction)) {
                missionFailed = true;
            }
            if (missionFailed) {
                break;
            }
            if (catIndex < catInstructionList.list.Count) {
                switch (catAction) {
                    case Utils.InstructionType.MOVE_U:
                        moveCat('U');
                        break;

                    case Utils.InstructionType.MOVE_L:
                        moveCat('L');
                        break;

                    case Utils.InstructionType.MOVE_R:
                        moveCat('R');
                        break;

                    case Utils.InstructionType.MOVE_D:
                        moveCat('D');
                        break;

                    case Utils.InstructionType.GRAB_U:
                        StartCoroutine(grabCat('U'));
                        break;

                    case Utils.InstructionType.GRAB_L:
                        StartCoroutine(grabCat('L'));
                        break;

                    case Utils.InstructionType.GRAB_R:
                        StartCoroutine(grabCat('R'));
                        break;

                    case Utils.InstructionType.GRAB_D:
                        StartCoroutine(grabCat('D'));
                        break;

                    case Utils.InstructionType.WAIT:
                        waitCat();
                        if (!catIsWaiting) continue;
                        break;
                }
                if (!catIsWaiting) {
                    catNextStep();
                }
            }
            if (dogIndex < dogInstructionList.list.Count) {
                switch (dogAction) {
                    case Utils.InstructionType.MOVE_U:
                        moveDog('U');
                        break;

                    case Utils.InstructionType.MOVE_L:
                        moveDog('L');
                        break;

                    case Utils.InstructionType.MOVE_R:
                        moveDog('R');
                        break;

                    case Utils.InstructionType.MOVE_D:
                        moveDog('D');
                        break;

                    case Utils.InstructionType.GRAB_U:
                        StartCoroutine(grabDog('U'));
                        break;

                    case Utils.InstructionType.GRAB_L:
                        StartCoroutine(grabDog('L'));
                        break;

                    case Utils.InstructionType.GRAB_R:
                        StartCoroutine(grabDog('R'));
                        break;

                    case Utils.InstructionType.GRAB_D:
                        StartCoroutine(grabDog('D'));
                        break;

                    case Utils.InstructionType.WAIT:
                        waitDog();
                        if (!dogIsWaiting) continue;
                        break;
                }
                if (!dogIsWaiting) {
                    dogNextStep();
                }
            }
            if (catIsWaiting && dogIndex >= dogInstructionList.list.Count) {
                missionFailed = true;
                missionResult = MissionResult.Condition.CAT_WAITING_FAIL;
                break;
            }
            if (dogIsWaiting && catIndex >= catInstructionList.list.Count) {
                missionFailed = true;
                missionResult = MissionResult.Condition.DOG_WAITING_FAIL;
                break;
            }
            yield return new WaitForSeconds(secondsPerMove + secondsBetweenMoves);
        }

        yield return new WaitForSeconds(1f);
        if (!missionFailed) {
            missionResult = MissionResult.checkMissionResult(puzzleManager, catIsHoldingKind, dogIsHoldingKind);
            if (missionResult != MissionResult.Condition.SUCCESS) {
                missionFailed = true;
            }
        }
        endMenu.SetActive(true);
        foreach (Text child in endMenu.GetComponentsInChildren<Text>()) {
            if (child.name == "ResultText") {
                child.text = MissionResult.conditionNames[missionResult];
            }
        }

        if (missionResult == MissionResult.Condition.SUCCESS) {
            SaveManager.SetFinishedMap(PuzzleManager.stageInfo.name.Substring("PuzzleStage".Length));
        }

        isInterpreting = false;
        yield break;
    }

    public void resetMissionState() {
        missionFailed = false;
        Destroy(catIsHolding);
        catIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
        catHolding.color = new Color(1, 1, 1, 0);
        Destroy(dogIsHolding);
        dogIsHoldingKind = PuzzleManager.PuzzleObject.NTH;
        dogHolding.color = new Color(1, 1, 1, 0);
        if (catArrow)
            catArrow.gameObject.SetActive(false);
        if (dogArrow)
            dogArrow.gameObject.SetActive(false);
        catIsWaiting = false;
        dogIsWaiting = false;
    }

    public void interpret() {
        if (!isInterpreting) {
            isInterpreting = true;

            StartCoroutine(interpretationEvent());
        }
    }

    private void Start() {
        isInterpreting = false;
    }
}
