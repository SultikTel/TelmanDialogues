using System.Collections.Generic;
using System.Linq;
using TelmanDialogues.Data;
using TelmanDialogues.Data.Error;
using TelmanDialogues.Dialogues;
using TelmanDialogues.Windows.Elements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace TelmanDialogues.Windows
{
    public class DialoguesEditorGraphView : GraphView
    {
        private Dictionary<string, DialoguesSystemNodeErrorData> _nodes;
        private DialoguesSystem _dialoguesSystem;
        private IMGUIContainer _inspector;
        private Vector2 _inspectorScroll;
        public DialogueSystemNode SelectedNode { get; private set; }

        public DialoguesEditorGraphView()
        {
            _nodes = new Dictionary<string, DialoguesSystemNodeErrorData>();

            AddManipulators();

            AddGridBG();

            AddStyles();

            OnElementsDeleted();

            AddToolBar();
        }

        public void Init(DialoguesSystem dialoguesSystem)
        {
            _dialoguesSystem = dialoguesSystem;
        }

        public void SelectNode(DialogueSystemNode node)
        {
            SelectedNode = node;
        }

        public void CreateInspector(VisualElement root)
        {
            _inspector = new IMGUIContainer(() =>
            {
                if (SelectedNode == null)
                    return;

                DrawNodeInspector(SelectedNode);
            });

            root.Add(_inspector);
        }

        private void DrawNodeInspector(DialogueSystemNode node)
        {
            _inspectorScroll = EditorGUILayout.BeginScrollView(_inspectorScroll);

            EditorGUILayout.Space(10);

            DialoguesBlock block = node.DialoguesBlock;

            if (block != null)
            {
                SerializedObject so = new SerializedObject(block);
                so.Update();

                SerializedProperty lines = so.FindProperty("_dialogueLines");
                EditorGUILayout.PropertyField(lines, true);

                so.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
        }

        #region Save

        private void Save()
        {
            _dialoguesSystem.Save(GetAllData());
        }

        private List<DialoguesSystemNodeData> GetAllData()
        {
            List<DialoguesSystemNodeData> result = new List<DialoguesSystemNodeData>();

            foreach (GraphElement element in graphElements)
            {
                if (element is DialogueSystemNode node)
                {
                    DialoguesSystemNodeData nodeData = new DialoguesSystemNodeData();

                    DialoguesBlock block = new DialoguesBlock();
                    //block.SetData(node.NodeName, node.DialoguesBlock.DialogueLines, null);
                    nodeData.SetValues(node.GetPosition().position, block);
                    result.Add(nodeData);
                }
            }

            Debug.Log(result.Count);

            return result;
        }

        #endregion

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
        #region NodeManipulations
        private DialogueSystemNode CreateNode(Vector2 position)
        {
            DialogueSystemNode node = new DialogueSystemNode();

            DialoguesBlock block = CreateDialoguesBlockAsset();

            node.Init(this, position, block);
            node.Draw();

            AddElement(node);

            AddNode(node);

            return node;
        }

        public void AddNode(DialogueSystemNode dialogueSystemNode)
        {
            string nodeName = dialogueSystemNode.DialoguesBlock.BlockName;

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
            string nodeName = dialogueSystemNode.DialoguesBlock.BlockName;

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

        private DialoguesBlock CreateDialoguesBlockAsset()
        {
            DialoguesBlock block = ScriptableObject.CreateInstance<DialoguesBlock>();
            block.SetData("NewNode", new(), new());
            string folderPath = _dialoguesSystem.DataFolderPath;

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{folderPath}/DialoguesBlock.asset"
            );

            AssetDatabase.CreateAsset(block, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return block;
        }

        #endregion

        #region Basics

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            Button saveButton = new Button(Save)
            {
                text = "Save"
            };

            toolbar.Add(saveButton);

            Add(toolbar);
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

                foreach (DialogueSystemNode node in nodesToDelete)
                {
                    if (node.DialoguesBlock != null)
                    {
                        string path = AssetDatabase.GetAssetPath(node.DialoguesBlock);

                        if (!string.IsNullOrEmpty(path))
                        {
                            AssetDatabase.DeleteAsset(path);
                        }
                    }

                    RemoveNode(node);
                    RemoveElement(node);
                }
            };
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

        #endregion
    }
}