using UnityEngine;

namespace Base
{
    public class Game : MonoBehaviour
    {
        public static Game Instance { get; private set; } = null;
        public DefRepositoryDef DefRepositoryDef;

        void Awake()
        {
            Instance = this;
        }
    }
}
