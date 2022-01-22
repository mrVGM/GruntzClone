using ScriptingLanguage.REPL;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ScriptingLanguage
{
    public class ScriptsConsole : MonoBehaviour
    {
        [SerializeField]
        private TextAsset ParserTable;
        public GameObject GraphicElements;
        public InputField InputField;
        public Text ConsoleOutput;
        public KeyCode[] KeyComboActivate = new KeyCode[] { KeyCode.Backslash };
        public KeyCode[] KeyComboDeactivate = new KeyCode[] { KeyCode.Escape };

        private static ScriptsConsole Instance = null;

        bool Active = false;

        REPL.REPL repl;
        REPL.REPL REPL
        {
            get
            {
                if (repl == null)
                {
                    var pt = Parser.ParserTable.Deserialize(ParserTable.bytes);
                    repl = new REPL.REPL(pt);
                }
                return repl;
            }
        }

        void Awake()
        {
            InputField.interactable = false;
            Show(false);
            Instance = this;
        }

        void Show(bool show)
        {
            var rect = GraphicElements.GetComponent<RectTransform>();
            if (show)
                rect.anchoredPosition = Vector2.zero;
            else
                rect.anchoredPosition = rect.rect.height * Vector2.up;

            Active = show;
        }

        void Update()
        {
            if (!Active && KeyComboActivate.All(x => Input.GetKey(x)))
            {
                Show(true);
                InputField.interactable = true;
                InputField.text = "";
                EventSystem.current.SetSelectedGameObject(InputField.gameObject);
                return;
            }

            if (Active && GraphicElements.activeSelf && KeyComboDeactivate.All(x => Input.GetKey(x)))
            {
                Show(false);
                InputField.interactable = false;
                return;
            }
        }

        void Print(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                ConsoleOutput.text += "\n";
                ConsoleOutput.text += $" {str}";
            }
        }

        public void HandleCommand()
        {
            string command = InputField.text;
            Print(command);
            var res = REPL.HandleCommand(command);
            foreach (var str in res)
            {
                Print(str);
            }

            InputField.text = "";
            InputField.Select();
            InputField.ActivateInputField();
        }

        [MetaCommand(Name = "change_font_size")]
        private static IEnumerable<string> ChangeFontSize(object context, IEnumerable<string> args)
        {
            if (Instance == null)
                yield break;

            string sizeArg = args.FirstOrDefault();
            if (string.IsNullOrEmpty(sizeArg))
                yield break;

            int size = int.Parse(sizeArg);
            if (size < 0)
                yield break;

            var texts = Instance.GraphicElements.GetComponentsInChildren<Text>();
            foreach (var text in texts)
            {
                text.fontSize = size;
            }
        }

        [MetaCommand(Name = "change_font_color")]
        private static IEnumerable<string> ChangeFontColor(object context, IEnumerable<string> args)
        {
            if (Instance == null)
                yield break;

            string colorArg = args.FirstOrDefault();
            if (string.IsNullOrEmpty(colorArg))
                yield break;

            var texts = Instance.GraphicElements.GetComponentsInChildren<Text>();
            void changeColor(Color color)
            {
                foreach (var text in texts)
                    text.color = color;
            }

            switch (colorArg)
            {
                case "black":
                    changeColor(Color.black);
                    break;
                case "white":
                    changeColor(Color.white);
                    break;
                case "red":
                    changeColor(Color.red);
                    break;
                case "yellow":
                    changeColor(Color.yellow);
                    break;
                case "green":
                    changeColor(Color.green);
                    break;
                case "cyan":
                    changeColor(Color.cyan);
                    break;
            }
        }

        [MetaCommand(Name = "cls")]
        private static IEnumerable<string> CLS(object context, IEnumerable<string> args)
        {
            if (Instance == null)
                yield break;

            Instance.ConsoleOutput.text = "";
        }
    }
}
