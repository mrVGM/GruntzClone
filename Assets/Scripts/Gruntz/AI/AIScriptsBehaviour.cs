using UnityEngine;

namespace Gruntz.AI
{
    public class AIScriptsBehaviour : MonoBehaviour
    {
        public GameObject ScriptsRoot;
        [TextArea(15, 20)]
        public string Script;
    }
}
