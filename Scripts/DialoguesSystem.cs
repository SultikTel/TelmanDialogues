using System.Collections.Generic;
using UnityEngine;

namespace TelmanDialogues.Dialogues
{
    [CreateAssetMenu(fileName = "DialoguesSystem", menuName = "Game/Dialogue System")]
    public class DialoguesSystem : ScriptableObject
    {
        [SerializeField] private List<string> _characters;
        public List<string> Characters => _characters;
    }
}