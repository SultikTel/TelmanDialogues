using System;
using System.Collections.Generic;
using System.Linq;
using TelmanDialogues.Data;
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
        private DialoguesSystem _dialoguesSystem;
        private IMGUIContainer _inspector;
        private Vector2 _inspectorScroll;
        private List<Edge> Edges => edges.ToList();
        private List<DialogueSystemNode> Nodes => nodes.ToList().Cast<DialogueSystemNode>().ToList();
        public DialogueSystemNode SelectedNode { get; private set; }

        public DialoguesEditorGraphView()
        {
            AddManipulators();

            AddGridBG();

            AddStyles();

            AddToolBar();

            //OnElementsDeleted();
        }

        public void Init(DialoguesSystem dialoguesSystem)
        {
            _dialoguesSystem = dialoguesSystem;
            LoadGraph();
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

            LinesQueue block = node.LinesQueue;

            if (block != null)
            {
                EditorGUILayout.LabelField("Block Name:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(node.BlockName);

                EditorGUILayout.Space(5);
                SerializedObject so = new SerializedObject(block);
                so.Update();

                SerializedProperty lines = so.FindProperty("_dialogueLines");
                EditorGUILayout.PropertyField(lines, true);

                so.ApplyModifiedProperties();
            }

            EditorGUILayout.EndScrollView();
        }

        #region Save and Load
        private void Save()
        {
            List<DialoguesNodeLinkData> allLinks = new List<DialoguesNodeLinkData>();
            foreach (Edge edge in Edges)
            {
                DialogueSystemNode outputNode = edge.output.node as DialogueSystemNode;
                DialogueSystemNode inputNode = edge.input.node as DialogueSystemNode;

                allLinks.Add(new DialoguesNodeLinkData(outputNode.GUID, edge.output.portName, inputNode.GUID));
            }

            List<DialoguesSystemNodeData> allNodes = new List<DialoguesSystemNodeData>();
            foreach (DialogueSystemNode dialogueSystemNode in Nodes)
            {
                allNodes.Add(new DialoguesSystemNodeData(dialogueSystemNode.GUID, dialogueSystemNode.GetPosition().position, dialogueSystemNode.BlockName));
            }

            _dialoguesSystem.Save(allLinks, allNodes);

#if UNITY_EDITOR
            EditorUtility.SetDirty(_dialoguesSystem);
            AssetDatabase.SaveAssets();
#endif

            ClearUnusedAssets();
        }

        private void ClearUnusedAssets()
        {
            string folderPath = _dialoguesSystem.DataFolderPath;
            HashSet<string> nodeGuids = new HashSet<string>(_dialoguesSystem.DialoguesSystemNodeDatas.Select(n => n.GUID));

            string[] assetGuids = AssetDatabase.FindAssets($"t:{nameof(LinesQueue)}", new[] { folderPath });

            foreach (string assetGuid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                LinesQueue asset = AssetDatabase.LoadAssetAtPath<LinesQueue>(path);

                if (asset == null)
                    continue;

                if (!nodeGuids.Contains(asset.name))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void LoadGraph()
        {
            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }

        private void ConnectNodes()
        {
            foreach (DialogueSystemNode node in Nodes)
            {
                List<DialoguesNodeLinkData> dialoguesNodeLinkDatas = _dialoguesSystem.NodeLinks.Where(x => x.BaseNodeGUID == node.GUID).ToList();

                foreach (DialoguesNodeLinkData link in dialoguesNodeLinkDatas)
                {
                    string targetNodeGUID = link.TargetNodeGUID;
                    DialogueSystemNode targetNode = Nodes.First(x => x.GUID == targetNodeGUID);
                    Port outPutPort = node.outputContainer.Query<Port>().ToList().FirstOrDefault(x => x.portName == link.TextValue && !x.connections.Any());
                    LinkNodes(outPutPort, (Port)targetNode.inputContainer[0]);
                }
            }
        }

        private void LinkNodes(Port outPut, Port input)
        {
            Edge edge = new Edge
            {
                output = outPut,
                input = input
            };

            edge?.output.Connect(edge);
            edge?.input.Connect(edge);

            Add(edge);
        }

        private void CreateNodes()
        {
            foreach (DialoguesSystemNodeData nodeData in _dialoguesSystem.DialoguesSystemNodeDatas)
            {
                CreateNode(Vector2.zero, nodeData, _dialoguesSystem.NodeLinks);
            }
        }

        private void ClearGraph()
        {
            foreach (DialogueSystemNode dialogueSystemNode in Nodes)
            {
                RemoveElement(dialogueSystemNode);
            }

            foreach (Edge edge in Edges)
            {
                RemoveElement(edge);
            }
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
                    if (node.LinesQueue != null)
                    {
                        string path = AssetDatabase.GetAssetPath(node.LinesQueue);

                        if (!string.IsNullOrEmpty(path))
                        {
                            AssetDatabase.DeleteAsset(path);
                        }
                    }
                    RemoveElement(node);
                }
            };
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
        private DialogueSystemNode CreateNode(Vector2 position, DialoguesSystemNodeData nodeData = null, List<DialoguesNodeLinkData> links = null)
        {
            DialogueSystemNode node = new DialogueSystemNode();

            string GUID;
            if (nodeData != null)
            {
                GUID = nodeData.GUID;
            }
            else
            {
                GUID = Guid.NewGuid().ToString();
            }

            LinesQueue linesQueue = CreateOrFindLinesQueue(GUID);

            node.Draw(this, position, linesQueue, GUID, nodeData, links);

            AddElement(node);

            return node;
        }

        private LinesQueue CreateOrFindLinesQueue(string guid)
        {
            string folderPath = _dialoguesSystem.DataFolderPath;

            if (!string.IsNullOrEmpty(guid))
            {
                string[] guids = AssetDatabase.FindAssets($"t:{nameof(LinesQueue)} {guid}", new[] { folderPath });

                if (guids.Length > 0)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    LinesQueue existing = AssetDatabase.LoadAssetAtPath<LinesQueue>(path);

                    if (existing != null)
                        return existing;
                }
            }

            LinesQueue lines = ScriptableObject.CreateInstance<LinesQueue>();
            lines.Init(new());

            string assetName = !string.IsNullOrEmpty(guid) ? guid : Guid.NewGuid().ToString();

            lines.name = assetName;

            string assetPath = AssetDatabase.GenerateUniqueAssetPath(
                $"{folderPath}/{assetName}.asset"
            );

            AssetDatabase.CreateAsset(lines, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return lines;
        }


        #endregion

        #region Basics

        private void AddToolBar()
        {
            Toolbar toolbar = new Toolbar();

            Button resetPositionButton = new Button(ResetPosition) { text = "ResetPosition" };
            Button saveButton = new Button(Save) { text = "Save" };

            toolbar.Add(resetPositionButton);
            toolbar.Add(saveButton);

            Add(toolbar);
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

        private void ResetPosition()
        {
            UpdateViewTransform(Vector3.zero, Vector3.one);
        }

        #endregion
    }
}