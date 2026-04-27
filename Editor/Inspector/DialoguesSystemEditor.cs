using TelmanDialogues.Dialogues;
using TelmanDialogues.Windows;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialoguesSystem))]
public class DialoguesSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Debug Log"))
        {
            DialoguesEditorWindow dialoguesEditorWindow = EditorWindow.GetWindow<DialoguesEditorWindow>("Dialogue Editor");
        }
    }
}