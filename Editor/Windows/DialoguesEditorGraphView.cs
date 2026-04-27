using System.Collections.Generic;
using System.Linq;
using TelmanDialogues.Data.Error;
using TelmanDialogues.Windows.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelmanDialogues.Windows
{
    public class DialoguesEditorGraphView : GraphView
    {
        private Dictionary<string, DialoguesSystemNodeErrorData> _nodes;

        public DialoguesEditorGraphView()
        {
            _nodes = new Dictionary<string, DialoguesSystemNodeErrorData>();

            AddManipulators();

            AddGridBG();

            AddStyles();

            OnElementsDeleted();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort.node == port.node) return;

                if (startPort.direction == port.direction) return;

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private void OnElementsDeleted()
        {
            deleteSelection += (operationName, askUser) =>
            {
                List<DialogueSystemNode> nodesToDelete = new List<DialogueSystemNode>();
                List<Edge> edgesToDelete = new List<Edge>();
                foreach (GraphElement element in selection)
                {
                    if (element is DialogueSystemNode node)
                    {
                        nodesToDelete.Add(node);

                        foreach (var port in node.Query<Port>().ToList())
                        {
                            edgesToDelete.AddRange(port.connections);
                        }
                    }
                    else if (element is Edge edge)
                    {
                        edgesToDelete.Add(edge);
                    }
                }

                foreach (Edge edge in edgesToDelete.Distinct())
                {
                    edge.input?.Disconnect(edge);
                    edge.output?.Disconnect(edge);
                    RemoveElement(edge);
                }

                foreach (var node in nodesToDelete)
                {
                    RemoveNode(node);
                    RemoveElement(node);
                }
            };
        }

        private DialogueSystemNode CreateNode(Vector2 position)
        {
            DialogueSystemNode node = new DialogueSystemNode();

            node.Init(this, position);
            node.Draw();

            AddElement(node);

            AddNode(node);

            return node;
        }

        public void AddNode(DialogueSystemNode dialogueSystemNode)
        {
            string nodeName = dialogueSystemNode.NodeName;

            if (!_nodes.ContainsKey(nodeName))
            {
                DialoguesSystemNodeErrorData dialoguesSystemNodeErrorData = new DialoguesSystemNodeErrorData();

                dialoguesSystemNodeErrorData.Nodes.Add(dialogueSystemNode);

                _nodes.Add(nodeName, dialoguesSystemNodeErrorData);

                return;
            }

            _nodes[nodeName].Nodes.Add(dialogueSystemNode);

            Color errorColor = _nodes[nodeName].ErrorData.Color;

            dialogueSystemNode.SetErrorStyle(errorColor);

            if (_nodes[nodeName].Nodes.Count == 2)
            {
                _nodes[nodeName].Nodes[0].SetErrorStyle(errorColor);
            }
        }

        public void RemoveNode(DialogueSystemNode dialogueSystemNode)
        {
            string nodeName = dialogueSystemNode.NodeName;

            _nodes[nodeName].Nodes.Remove(dialogueSystemNode);

            dialogueSystemNode.ResetStyle();

            if (_nodes[nodeName].Nodes.Count == 1)
            {
                _nodes[nodeName].Nodes[0].ResetStyle();

                return;
            }

            if (_nodes[nodeName].Nodes.Count == 0)
            {
                _nodes.Remove(nodeName);
            }
        }

        private void AddManipulators()
        {
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(CreateNodeContextualMenu());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private IManipulator CreateNodeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent =>
                {
                    menuEvent.menu.AppendAction("Add Node", actionEvent =>
                    {
                        Vector2 mousePosition = actionEvent.eventInfo.localMousePosition;

                        Vector2 graphPosition = contentViewContainer.WorldToLocal(mousePosition);

                        AddElement(CreateNode(graphPosition));
                    });
                });

            return contextualMenuManipulator;
        }

        private void AddStyles()
        {
            StyleSheet dialoguesEditorStyleSheet = Resources.Load<StyleSheet>("DialoguesEditorStyleSheet");
            StyleSheet dialoguesNodesStyles = Resources.Load<StyleSheet>("DialoguesNodesStyles");

            styleSheets.Add(dialoguesEditorStyleSheet);
            styleSheets.Add(dialoguesNodesStyles);
        }

        private void AddGridBG()
        {
            GridBackground gridBG = new GridBackground();

            gridBG.StretchToParentSize();

            Insert(0, gridBG);
        }
    }
}