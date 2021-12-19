using System;
using UnityEngine;

namespace Gruntz.AI
{
    public class AIScript : MonoBehaviour
    {
        [Serializable]
        public class Prop
        {
            public string Name;
            public GameObject GameObject;
        }

        public Prop[] Props;

        [TextArea(15, 20)]
        public string Script;
    }
}
