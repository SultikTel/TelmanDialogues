using System;
using System.Collections.Generic;
using UnityEngine;

namespace TelmanDialogues.Dialogues
{
    public class DialoguesBlock : ScriptableObject
    {
        [SerializeField] private string _blockName;
        public string BlockName => _blockName;
        [SerializeField] private List<DialogueLine> _dialogueLines;
        public List<DialogueLine> DialogueLines => _dialogueLines;

        [SerializeField] private List<DialogueChoice> _choices;
        public List<DialogueChoice> Choices => _choices;

        public void SetData(string blockName, List<DialogueLine> dialogueLines, List<DialogueChoice> choices)
        {
            _blockName = blockName;
            _dialogueLines = dialogueLines;
            _choices = choices;
        }
    }

    [Serializable]
    public class DialogueChoice
    {
        [SerializeField] private string _text;
        public string Text => _text;

        [SerializeField] private DialoguesBlock _nextBlock;
        public DialoguesBlock NextBlock => _nextBlock;
    }

    [Serializable]
    public class DialogueLine
    {
        [SerializeField] private string _character;
        public string Character => _character;
        [SerializeField] private string _line;
        public string Line => _line;
        [SerializeField] private List<string> _events;
        [SerializeField] public List<string> Events => _events;
    }
}