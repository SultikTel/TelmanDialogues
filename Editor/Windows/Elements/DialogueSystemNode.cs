using TelmanDialogues.Dialogues;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelmanDialogues.Windows.Elements
{
    public class DialogueSystemNode : Node
    {
        private Color _defaultBGColor;
        private DialoguesEditorGraphView _graphView;
        [SerializeField] private string _nodeName;
        public string NodeName => _nodeName;
        [SerializeField] private DialoguesBlock _dialoguesBlock;
        public DialoguesBlock DialoguesBlock => _dialoguesBlock;

        public void Init(DialoguesEditorGraphView dialoguesEditorGraphView, Vector2 position)
        {
            _graphView = dialoguesEditorGraphView;

            _nodeName = "Name";

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

            Port outputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "";
            outputPort.AddToClassList("output-port");

            TextField nodeName = new TextField()
            {
                value = _nodeName
            };

            nodeName.RegisterValueChangedCallback(evt =>
            {
                _graphView.RemoveNode(this);

                _nodeName = evt.newValue;

                _graphView.AddNode(this);
            });

            nodeName.AddToClassList("node-name");

            inputContainer.Clear();
            outputContainer.Clear();

            inputContainer.Add(inputPort);
            inputContainer.Add(nodeName);
            inputContainer.Add(outputPort);

            titleContainer.style.display = DisplayStyle.None;

            RegisterCallback<MouseDownEvent>(evt =>
            {
                _graphView.SelectNode(this);
            });

            RefreshExpandedState();
        }
        public void SetDialoguesBlock(DialoguesBlock block)
        {
            _dialoguesBlock = block;
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