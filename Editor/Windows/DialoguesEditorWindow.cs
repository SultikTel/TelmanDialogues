using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelmanDialogues.Windows
{
    public class DialoguesEditorWindow : EditorWindow
    {

        [MenuItem("Tools/Dialogue Editor")]
        public static void Open()
        {
            GetWindow<DialoguesEditorWindow>("Dialogue Editor");
        }

        private void CreateGUI()
        {
            AddGraphView();

            AddStyles();
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("DialoguesEditorVariables");

            rootVisualElement.styleSheets.Add(styleSheet);
        }

        private void AddGraphView()
        {
            DialoguesEditorGraphView dialoguesEditorGraphView = new DialoguesEditorGraphView();

            dialoguesEditorGraphView.StretchToParentSize();

            rootVisualElement.Add(dialoguesEditorGraphView);
        }
    }
}