using System.IO;
using UnityEditor;
using UnityEngine;

namespace ScriptingLanguage
{
#if UNITY_EDITOR
    public class REPLWindow : EditorWindow
    {
        private static REPLWindow Instance;
        public string Script;

        [SerializeField]
        private TextAsset ParserTable;
        
        private ScriptingLanguage.REPL.REPL repl;
        private ScriptingLanguage.REPL.REPL REPL
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

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            Script = GUILayout.TextField(Script);
            if (GUILayout.Button("Choose Script")) 
            {
                Script = EditorUtility.OpenFilePanel("Choose Script", "", "txt");
            }
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Execute")) 
            {
                if (string.IsNullOrEmpty(Script))
                {
                    return;
                }

                var lines = File.ReadAllLines(Script);

                foreach (var line in lines) 
                {
                    var toPrint = REPL.HandleCommand(line);
                    foreach (var str in toPrint) 
                    {
                        Debug.Log(str);
                    }
                }
            }
        }

        [MenuItem("Window/REPL")]
        public static void ShowWindow()
        {
            var window = GetWindow<REPLWindow>("REPL Console");
            if (Instance == null) 
            {
                Instance = window;
            }
        }
    }
#endif
}