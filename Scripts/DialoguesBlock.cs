using System;
using System.Collections.Generic;
using UnityEngine;

namespace TelmanDialogues.Dialogues
{
    [Serializable]
    public class DialoguesBlock
    {
        [SerializeField] private string _blockName;
        public string BlockName => _blockName;
        [SerializeField] private LinesQueue _linesQueue;
        public LinesQueue LinesQueue => _linesQueue;

        [SerializeField] private List<DialogueChoice> _choices;
        public List<DialogueChoice> Choices => _choices;

        public void SetData(string blockName, LinesQueue linesQueue, List<DialogueChoice> choices)
        {
            _blockName = blockName;
            _linesQueue = linesQueue;
            _choices = choices;
        }

        public void SetName(string name)
        {
            _blockName = name;
        }
    }
    [Serializable]
    public class DialogueChoice
    {
        [SerializeField] private string _text;
        public string Text => _text;
        [SerializeField] private DialoguesBlock _nextBlock;
        public DialoguesBlock NextBlock => _nextBlock;

        public DialogueChoice(string text, DialoguesBlock nextBlock)
        {
            _text = text;
            _nextBlock = nextBlock;
        }

        public void SetText(string newText)
        {
            _text = newText;
        }

        public void SetNextBlock(DialoguesBlock newDialogueTextBlock)
        {
            _nextBlock = newDialogueTextBlock;
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
        [SerializeField] public List<string> Events => _events;
    }
}