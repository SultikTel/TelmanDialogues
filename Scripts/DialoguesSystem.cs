using System.Collections.Generic;
using TelmanDialogues.Data;
using UnityEditor;
using UnityEngine;

namespace TelmanDialogues.Dialogues
{
    public class DialoguesSystem : ScriptableObject
    {
        private List<DialoguesSystemNodeData> _dialoguesSystemNodeDatas;
        public List<DialoguesSystemNodeData> DialoguesSystemNodeDatas => _dialoguesSystemNodeDatas;

        [SerializeField, HideInInspector] private string _dataFolderPath;
        public string DataFolderPath => _dataFolderPath;

        public void SetDataFolderGuid(string guid)
        {
            _dataFolderPath = guid;
        }

        public void Save(List<DialoguesSystemNodeData> newDatas)
        {
            _dialoguesSystemNodeDatas = newDatas;
        }
    }
}