using TelmanDialogues.Dialogues;
using TelmanDialogues.Windows;
using UnityEditor;
using UnityEngine;

namespace TelmanDialogues
{
    [CustomEditor(typeof(DialoguesSystem))]
    public class DialoguesSystemEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("OpenEditWindow"))
            {
                if (target is DialoguesSystem dialoguesSystem)
                {
                    DialoguesEditorWindow dialoguesEditorWindow = EditorWindow.GetWindow<DialoguesEditorWindow>("Dialogue Editor");

                    dialoguesEditorWindow.Open(dialoguesSystem);
                }
            }

            DrawDefaultInspector();
        }
    }
}