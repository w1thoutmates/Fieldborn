using UnityEngine;
using UnityEditor;
using Mono.Cecil;
using Codice.Client.Common.GameUI;

[CustomEditor(typeof(ShapeData), false)]
[CanEditMultipleObjects]
[System.Serializable]
public class ShapeDataDrawer : Editor
{
    private ShapeData ShapeDataInstance => target as ShapeData;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        ClearBoardButton();
        EditorGUILayout.Space();

        DrawColumnsInputFields();
        EditorGUILayout.Space();

        if (ShapeDataInstance.board != null && ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            DrawBoardTable();
        }

        serializedObject.ApplyModifiedProperties();

        if(GUI.changed)
        {
            EditorUtility.SetDirty(ShapeDataInstance);
        }
    }

    private void ClearBoardButton()
    {
        if(GUILayout.Button("Clear Board"))
        {
            ShapeDataInstance.Clear();
        }
    }
    
    private void DrawColumnsInputFields()
    {
        var columnsTemp = ShapeDataInstance.columns;
        var rowsTemp = ShapeDataInstance.rows;

        ShapeDataInstance.columns = EditorGUILayout.IntField("Columns", ShapeDataInstance.columns);
        ShapeDataInstance.rows = EditorGUILayout.IntField("Rows", ShapeDataInstance.rows);

        if((ShapeDataInstance.columns != columnsTemp) || (ShapeDataInstance.rows != rowsTemp) &&
            ShapeDataInstance.columns > 0 && ShapeDataInstance.rows > 0)
        {
            ShapeDataInstance.CreateNewBoard();
        }
    }

    private void DrawBoardTable()
    {
        for (var row = 0; row < ShapeDataInstance.rows; row++)
        {
            EditorGUILayout.BeginHorizontal(); 

            for (var col = 0; col < ShapeDataInstance.columns; col++)
            {
                bool currentValue = ShapeDataInstance.board[row].column[col];

                bool newValue = GUILayout.Toggle(currentValue, GUIContent.none, "Button", GUILayout.Width(25), GUILayout.Height(25));
                ShapeDataInstance.board[row].column[col] = newValue;
            }

            EditorGUILayout.EndHorizontal(); 
        }
    }
}
