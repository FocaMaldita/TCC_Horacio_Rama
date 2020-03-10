using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PuzzleStageScriptableObject))]
public class PuzzleStageEditor : Editor {

    public override void OnInspectorGUI() {

        PuzzleStageScriptableObject ps = (PuzzleStageScriptableObject)target;

        ps.rowCount = Mathf.Max(1, EditorGUILayout.IntField("Row Count:", ps.rowCount));
        ps.colCount = Mathf.Max(1, EditorGUILayout.IntField("Col Count:", ps.colCount));

        base.OnInspectorGUI();

        resize(ps);

        for (int i = ps.rowCount - 1; i >= 0; i--) {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < ps.colCount; j++) {
                ps.matrix[j].entries[i] = (PuzzleManager.PuzzleObject)EditorGUILayout.EnumPopup(ps.matrix[j].entries[i]);
            }
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Set dirty")) {
            EditorUtility.SetDirty(ps);
        }
    }

    void resize(PuzzleStageScriptableObject ps) {
        if (ps.matrix == null) {
            ps.matrix = new PuzzleStageScriptableObject.Row[ps.colCount];
            for (int i = 0; i < ps.colCount; i++) {
                ps.matrix[i] = new PuzzleStageScriptableObject.Row();
                ps.matrix[i].entries = new PuzzleManager.PuzzleObject[ps.rowCount];
            }
        } else {
            if (ps.matrix.Length != ps.rowCount || ps.matrix[0].entries.Length != ps.colCount) {
                var new_matrix = new PuzzleStageScriptableObject.Row[ps.colCount];
                for (int i = 0; i < ps.colCount; i++) {
                    new_matrix[i] = new PuzzleStageScriptableObject.Row();
                    new_matrix[i].entries = new PuzzleManager.PuzzleObject[ps.rowCount];
                }

                for (int i = 0; i < ps.colCount && i < ps.matrix.Length; i++) {
                    for (int j = 0; j < ps.rowCount && j < ps.matrix[0].entries.Length; j++) {
                        new_matrix[i].entries[j] = ps.matrix[i].entries[j];
                    }
                }

                ps.matrix = new_matrix;
            }
        }
    }
}
