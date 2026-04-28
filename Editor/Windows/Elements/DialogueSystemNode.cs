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

            _defaultBGColor = new Color(29f / 255, 29f / 255, 30f / 255);
        }

        public void Draw()
        {
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
            titleContainer.Insert(0, nodeName);

            Button addChoiceButton = new Button(() =>
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                choicePort.portName = "";

                Button deleteChoice = new Button()
                {
                    text = "X"
                };

                TextField choiceTextField = new TextField()
                {
                    value = "NewChoice"
                };
                choiceTextField.style.flexGrow = 1;
                choiceTextField.style.flexShrink = 1;
                choiceTextField.style.maxWidth = 100;

                choicePort.Add(choiceTextField);
                choicePort.Add(deleteChoice);

                outputContainer.Add(choicePort);

                DialogueChoice dialogueChoice = new DialogueChoice(choiceTextField.value, null);

                _dialoguesBlock.Choices.Add(dialogueChoice);
                choiceTextField.RegisterValueChangedCallback(evt =>
                {
                    dialogueChoice.SetText(evt.newValue);
                });

                RefreshExpandedState();
            })
            {
                text = "Add Choice"
            };

            mainContainer.Insert(1, addChoiceButton);

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Connections";
            inputContainer.Add(inputPort);

            foreach (DialogueChoice dialogueChoice in _dialoguesBlock.Choices)
            {
                Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                choicePort.portName = "";

                Button deleteChoice = new Button()
                {
                    text = "X"
                };

                TextField choiceTextField = new TextField()
                {
                    value = dialogueChoice.Text
                };

                choicePort.Add(choiceTextField);
                choicePort.Add(deleteChoice);

                outputContainer.Add(choicePort);
            }

            RegisterCallback<MouseDownEvent>(evt =>
            {
                _graphView.SelectNode(this);
            });

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