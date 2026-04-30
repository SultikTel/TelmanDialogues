using System;
using System.Collections.Generic;
using System.Linq;
using TelmanDialogues.Data;
using TelmanDialogues.Dialogues;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelmanDialogues.Windows.Elements
{
    public class DialogueSystemNode : Node
    {
        private string _GUID;
        public string GUID => _GUID;
        private LinesQueue _linesQueue;
        public LinesQueue LinesQueue => _linesQueue;
        private string _blockName;
        public string BlockName => _blockName;

        public void Draw(DialoguesEditorGraphView graphView, Vector2 position, LinesQueue linesQueue, DialoguesSystemNodeData dialoguesSystemNodeData = null, List<DialoguesNodeLinkData> links = null)
        {
            TextField nodeName = new TextField();

            if (dialoguesSystemNodeData == null)
            {
                SetPosition(new Rect(position, Vector2.zero));
                _GUID = Guid.NewGuid().ToString();
                nodeName.value = "NewNode";
            }
            else
            {
                SetPosition(new Rect(dialoguesSystemNodeData.Position, Vector2.zero));
                _GUID = dialoguesSystemNodeData.GUID;
                nodeName.value = dialoguesSystemNodeData.Name;

                if (links != null)
                {
                    List<string> result = links.Where(l => l.BaseNodeGUID == _GUID).Select(l => l.TextValue).ToList();
                    foreach (string portName in result)
                    {
                        CreatePort(graphView, portName);
                    }
                }
            }

            _linesQueue= linesQueue;

            _blockName = nodeName.value;

            nodeName.RegisterValueChangedCallback(evt => { _blockName = evt.newValue; });

            titleContainer.Insert(0, nodeName);

            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "Connections";
            inputContainer.Add(inputPort);

            Button addChoiceButton = new Button(() => { CreatePort(graphView); }) { text = "Add Choice" };

            mainContainer.Insert(1, addChoiceButton);

            RegisterCallback<MouseDownEvent>(evt => { graphView.SelectNode(this); });
            RefreshExpandedState();
        }

        private Port CreatePort(DialoguesEditorGraphView editorGraphView, string name = null)
        {
            Port choicePort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));

            choicePort.contentContainer.Q<Label>().style.display = DisplayStyle.None;

            Button deleteChoice = new Button()
            {
                text = "X"
            };

            TextField choiceTextField = new TextField();

            if (name != null)
            {
                choiceTextField.value = name;
            }
            else
            {
                choiceTextField.value = "NewChoice";
            }

            choicePort.portName = choiceTextField.value;

            choiceTextField.RegisterValueChangedCallback(evt =>
            {
                choicePort.portName = evt.newValue;
            });

            choiceTextField.style.flexGrow = 1;
            choiceTextField.style.flexShrink = 1;
            choiceTextField.style.maxWidth = 100;

            deleteChoice.clicked += () =>
            {
                var edgesToRemove = choicePort.connections.ToList();

                foreach (var edge in edgesToRemove)
                {
                    edge.input?.Disconnect(edge);
                    edge.output?.Disconnect(edge);

                    editorGraphView.RemoveElement(edge);
                }

                outputContainer.Remove(choicePort);

                RefreshExpandedState();
            };

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoice);

            outputContainer.Add(choicePort);

            RefreshExpandedState();

            return choicePort;
        }
    }
}