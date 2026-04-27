using System.Collections.Generic;
using System.Linq;
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

            DialogueBlockEnd currentType = DialogueBlockEnd.End;

            DropdownField outputTypeDropdown = new DropdownField(
                "Output Type",
                new List<string> { "End", "FreeChoice", "ForcedChoice" },
                (int)currentType
            );

            outputTypeDropdown.RegisterValueChangedCallback(evt =>
            {
                currentType = (DialogueBlockEnd)outputTypeDropdown.index;

                bool showOutput = currentType != DialogueBlockEnd.End;

                if (!showOutput)
                {
                    foreach (Edge edge in outputPort.connections.ToList())
                    {
                        edge.input.Disconnect(edge);
                        edge.output.Disconnect(edge);
                        edge.RemoveFromHierarchy();
                    }
                }

                outputPort.style.display = showOutput ? DisplayStyle.Flex : DisplayStyle.None;
            });

            outputPort.style.display = DisplayStyle.None;

            inputContainer.Clear();
            outputContainer.Clear();

            inputContainer.Add(inputPort);
            inputContainer.Add(nodeName);
            inputContainer.Add(outputTypeDropdown);

            inputContainer.Add(outputPort);

            titleContainer.style.display = DisplayStyle.None;

            RefreshExpandedState();
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = default;
        }
    }
}