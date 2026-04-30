using TelmanDialogues.Dialogues;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelmanDialogues.Windows
{
    public class DialoguesEditorWindow : EditorWindow
    {
        private DialoguesEditorGraphView _graphView;
        private DialoguesSystem _dialogueSystem;
        public DialoguesSystem DialogueSystem => _dialogueSystem;

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void CreateGUI()
        {
            AddStyles();

            CreateLayout();
        }

        public void Open(DialoguesSystem system)
        {
            _dialogueSystem = system;

            GetWindow<DialoguesEditorWindow>("Dialogue Editor");

            _graphView.Init(system);
        }

        private void OnSelectionChanged()
        {
            if (Selection.activeObject is DialoguesSystem system)
            {
                Open(system);
            }
        }
        #region Start Basic
        private void CreateLayout()
        {
            VisualElement root = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1
                }
            };

            rootVisualElement.Add(root);

            if (_graphView == null)
                _graphView = new DialoguesEditorGraphView();

            _graphView.style.flexGrow = 1;
            root.Add(_graphView);
        }

        private void AddStyles()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>("DialoguesEditorVariables");

            if (styleSheet != null)
                rootVisualElement.styleSheets.Add(styleSheet);
        }
        #endregion
    }
}