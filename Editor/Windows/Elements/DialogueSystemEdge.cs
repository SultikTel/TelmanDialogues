using TelmanDialogues.Dialogues;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace TelmanDialogues.Windows.Elements
{
    public class DialogueSystemEdge : Edge
    {
        [SerializeField] private string _text;
        public string Text => _text;

        [SerializeField] private DialoguesBlock _nextBlock;
        public DialoguesBlock NextBlock => _nextBlock;
    }
}