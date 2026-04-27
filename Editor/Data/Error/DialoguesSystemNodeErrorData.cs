using System.Collections.Generic;
using TelmanDialogues.Windows.Elements;
using UnityEngine;

namespace TelmanDialogues.Data.Error
{
    public class DialoguesSystemNodeErrorData
    {
        public DialoguesSystemErrorData ErrorData { get; private set; }
        public List<DialogueSystemNode> Nodes { get; private set; }

        public DialoguesSystemNodeErrorData()
        {
            ErrorData = new DialoguesSystemErrorData();
            Nodes = new List<DialogueSystemNode>();
        }
    }
}