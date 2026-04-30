using System;
using UnityEngine;

namespace TelmanDialogues.Data
{
    [Serializable]
    public class DialoguesSystemNodeData
    {
        [SerializeField] private string _name;
        public string Name => _name;
        [SerializeField] private Vector2 _position;
        public Vector2 Position => _position;

        [SerializeField] private string _GUID;
        public string GUID => _GUID;

        public DialoguesSystemNodeData(string gUID, Vector2 position, string name)
        {
            _GUID = gUID;
            _position = position;
            _name = name;
        }
    }

    [Serializable]
    public class DialoguesNodeLinkData
    {
        [SerializeField] private string _baseNodeGUID;
        public string BaseNodeGUID => _baseNodeGUID;
        [SerializeField] private string _textValue;
        public string TextValue => _textValue;
        [SerializeField] private string _targetNodeGUID;
        public string TargetNodeGUID => _targetNodeGUID;

        public DialoguesNodeLinkData(string baseNodeGUID, string textValue, string targetNodeGUID)
        {
            _baseNodeGUID = baseNodeGUID;
            _textValue = textValue;
            _targetNodeGUID = targetNodeGUID;
        }
    }
}