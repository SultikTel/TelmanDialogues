using UnityEngine;

namespace TelmanDialogues.Data.Error
{
    public class DialoguesSystemErrorData
    {
        public Color Color { get; private set; }

        public DialoguesSystemErrorData()
        {
            GenerateRandomColor();
        }

        private void GenerateRandomColor()
        {
            Color = new Color32(
                    (byte)Random.Range(65, 256),
                    (byte)Random.Range(50, 176),
                    (byte)Random.Range(50, 176),
                    255
                );
        }
    }
}