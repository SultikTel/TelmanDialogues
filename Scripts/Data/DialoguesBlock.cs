using System;
using System.Collections.Generic;
using UnityEngine;

namespace TelmanDialogues.Dialogues
{
    [Serializable]
    public class DialoguesBlock
    {
        [SerializeField] private string _guid;
        public string GUID => _guid;

        [SerializeField] private string _blockName;
        public string BlockName => _blockName;

        [SerializeField] private List<DialogueLine> _lines;
        public List<DialogueLine> Lines => _lines;

        [SerializeField] private List<DialogueChoice> _choices;
        public List<DialogueChoice> Choices => _choices;

        public void SetData(string guid, string blockName, List<DialogueLine> lines, List<DialogueChoice> choices)
        {
            _guid = guid;
            _blockName = blockName;
            _lines = lines;
            _choices = choices;
        }
    }

    [Serializable]
    public class DialogueChoice
    {
        [SerializeField] private string _text;
        public string Text => _text;

        [SerializeField] private string _nextBlockGUID;
        public string NextBlockGUID => _nextBlockGUID;

        public DialogueChoice(string text, string nextBlockGUID)
        {
            _text = text;
            _nextBlockGUID = nextBlockGUID;
        }
    }

    [Serializable]
    public class DialogueLine
    {
        [SerializeField] private string _character;
        public string Character => _character;
        [SerializeField] private string _line;
        public string Line => _line;
        [SerializeField] private List<string> _events;
        public List<string> Events => _events;
    }
}