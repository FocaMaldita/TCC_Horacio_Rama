using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructionNodeLoop : MonoBehaviour {
    public int origin;
    public int loopSize;

    public PuzzleManager puzzleManager;

    private LoopInfo info;

    public void recoverSize() {
        info = GetComponentInChildren<LoopInfo>();
        if (info) {
            origin = info.origin;
            loopSize = info.amount;
        }
    }

    private void Update() {
        if (puzzleManager.phase == 1 && !Interpreter.isInterpreting) {
            recoverSize();
        }
    }
}
