using TelmanDialogues.Dialogues;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelmanDialogues.Windows.Elements
{
    public class DialogueSystemNode : Node
    {
        private Color _defaultBGColor;
        private DialoguesEditorGraphView _graphView;
        [SerializeField] private DialoguesBlock _dialoguesBlock;
        public DialoguesBlock DialoguesBlock => _dialoguesBlock;

        public void Init(DialoguesEditorGraphView dialoguesEditorGraphView, Vector2 position, DialoguesBlock block)
        {
            _graphView = dialoguesEditorGraphView;

            _dialoguesBlock = block;
            SetPosition(new Rect(position, Vector2.zero));

            AddToClassList("node-root");

            _defaultBGColor = new Color(29f / 255, 29f / 255, 30f / 255);
        }

        public void Draw()
        {
            AddToClassList("node-root");

            Port inputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "";
            inputPort.AddToClassList("input-port");

            TextField nodeName = new TextField()
            {
                value = _dialoguesBlock.BlockName
            };

            nodeName.RegisterValueChangedCallback(evt =>
            {
                _graphView.RemoveNode(this);
                _dialoguesBlock.SetName(evt.newValue);
                _graphView.AddNode(this);
            });

            nodeName.AddToClassList("node-name");

            inputContainer.Clear();
            outputContainer.Clear();

            inputContainer.Add(inputPort);
            inputContainer.Add(nodeName);

            if (_dialoguesBlock != null && _dialoguesBlock.Choices != null)
            {
                for (int i = 0; i < _dialoguesBlock.Choices.Count; i++)
                {
                    int index = i;
                    DialogueChoice choice = _dialoguesBlock.Choices[index];

                    Port outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                    outputPort.portName = "";

                    Label label = new Label(choice.Text);
                    label.style.flexGrow = 1;

                    Button deleteButton = new Button(() =>
                    {
                        _dialoguesBlock.Choices.RemoveAt(index);
                        Draw();
                    })
                    {
                        text = "x"
                    };

                    deleteButton.style.width = 18;
                    deleteButton.style.height = 18;

                    VisualElement container = new VisualElement();
                    container.style.flexDirection = FlexDirection.Row;
                    container.style.alignItems = Align.Center;
                    container.style.flexGrow = 1;

                    container.Add(outputPort);
                    container.Add(label);
                    container.Add(deleteButton);

                    outputContainer.Add(container);
                }
            }

            titleContainer.style.display = DisplayStyle.None;

            RegisterCallback<MouseDownEvent>(evt =>
            {
                _graphView.SelectNode(this);
            });

            Button addChoiceButton = new Button(() =>
            {
                if (_dialoguesBlock == null)
                    return;

                Undo.RecordObject(_dialoguesBlock, "Add Choice");

                _dialoguesBlock.Choices.Add(new DialogueChoice());

                EditorUtility.SetDirty(_dialoguesBlock);

                Draw();
            })
            {
                text = "Add Choice"
            };

            extensionContainer.Add(addChoiceButton);

            RefreshExpandedState();
        }
        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = _defaultBGColor;
        }
    }
}