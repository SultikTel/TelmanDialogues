using System.Collections.Generic;
using UnityEngine;

namespace TelmanDialogues.Dialogues
{
    public class LinesQueue : ScriptableObject
    {
        [SerializeField] private List<DialogueLine> _dialogueLines;
        public List<DialogueLine> DialogueLines => _dialogueLines;

        public void Init(List<DialogueLine> dialogueLines)
        {
            _dialogueLines = dialogueLines;
        }
    }
}