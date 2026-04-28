using TelmanDialogues.Dialogues;
using UnityEngine;

namespace TelmanDialogues.Data
{
    public class DialoguesSystemNodeData
    {
        [SerializeField] private Vector2 _position;
        public Vector2 Position => _position;

        [SerializeField] private DialoguesBlock _dialoguesBlock;
        public DialoguesBlock DialoguesBlock => _dialoguesBlock;

        public void SetValues(Vector2 position, DialoguesBlock dialoguesBlock)
        {
            _position = position;
            _dialoguesBlock = dialoguesBlock;
        }
    }
}