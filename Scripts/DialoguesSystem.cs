using System.Collections.Generic;
using System.Linq;
using TelmanDialogues.Data;
using UnityEditor;
using UnityEngine;

namespace TelmanDialogues.Dialogues
{
    public class DialoguesSystem : ScriptableObject
    {
        [SerializeField, HideInInspector] private string _dataFolderPath;
        public string DataFolderPath => _dataFolderPath;

        [SerializeField] private List<DialoguesNodeLinkData> _nodeLinks = new();
        public List<DialoguesNodeLinkData> NodeLinks => _nodeLinks;

        [SerializeField] private List<DialoguesSystemNodeData> _dialogueNodeData = new();
        public List<DialoguesSystemNodeData> DialoguesSystemNodeDatas => _dialogueNodeData;

        [SerializeField] private List<DialoguesBlock> _dialoguesPure = new();
        public List<DialoguesBlock> DialoguesPure => _dialoguesPure;

        public void SetDataFolderGuid(string guid)
        {
            _dataFolderPath = guid;
        }

        public void Save(
            List<DialoguesNodeLinkData> nodeLinks,
            List<DialoguesSystemNodeData> dialogueNodeData
        )
        {
            _nodeLinks = nodeLinks;
            _dialogueNodeData = dialogueNodeData;

            BuildPureDialogues();
        }

        private void BuildPureDialogues()
        {
            _dialoguesPure.Clear();

            Dictionary<string, DialoguesBlock> map = new();

            foreach (var node in _dialogueNodeData)
            {
                LinesQueue queue = FindLinesQueue(node.GUID);

                var block = new DialoguesBlock();
                block.SetData(
                    node.Name,
                    queue != null ? queue.DialogueLines.ToList() : new List<DialogueLine>(),
                    new List<DialogueChoice>()
                );

                map[node.GUID] = block;
            }

            foreach (var link in _nodeLinks)
            {
                if (!map.ContainsKey(link.BaseNodeGUID)) continue;

                map[link.BaseNodeGUID].Choices.Add(
                    new DialogueChoice(
                        link.TextValue,
                        link.TargetNodeGUID 
                    )
                );
            }

            _dialoguesPure = map.Values.ToList();
        }

        private LinesQueue FindLinesQueue(string guid)
        {
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets(
                $"t:{nameof(LinesQueue)} {guid}",
                new[] { _dataFolderPath }
            );

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<LinesQueue>(path);
            }
#endif
            return null;
        }
    }
}