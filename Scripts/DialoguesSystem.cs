using System.Collections.Generic;
using TelmanDialogues.Data;
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

        public void SetDataFolderGuid(string guid)
        {
            _dataFolderPath = guid;
        }

        public void Save(List<DialoguesNodeLinkData> nodeLinks, List<DialoguesSystemNodeData> dialogueNodeData)
        {
            _nodeLinks = nodeLinks;
            _dialogueNodeData = dialogueNodeData;
        }
    }
}