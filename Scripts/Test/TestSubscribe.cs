using TelmanDialogues.Controller;
using UnityEngine;

namespace TelmanDialogues.Test
{
    public class TestSubscribe : MonoBehaviour
    {
        private void OnEnable()
        {
            DialoguesController.OnDialogueEvent += HandleEvent;
        }

        private void OnDisable()
        {
            DialoguesController.OnDialogueEvent -= HandleEvent;
        }

        private void HandleEvent(string evt)
        {
            if (evt == "Hello")
            {
                Debug.Log("Hello event triggered");
            }
        }
    }
}