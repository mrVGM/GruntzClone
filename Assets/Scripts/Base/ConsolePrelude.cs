using ScriptingLanguage;
using UnityEngine;

namespace Base
{
    public class ConsolePrelude : MonoBehaviour
    {
        public string[] PreludeCommands;
        private void Start()
        {
            var console = GetComponent<ScriptsConsole>();
            if (console == null)
            {
                return;
            }
            foreach (var command in PreludeCommands)
            {
                console.InputField.text = command;
                console.HandleCommand();
            }

            console.InputField.Select();
            console.InputField.ActivateInputField();
        }
    }
}
