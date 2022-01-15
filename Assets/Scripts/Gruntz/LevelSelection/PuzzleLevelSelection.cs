using UnityEngine;

namespace Gruntz
{
    public class PuzzleLevelSelection : MonoBehaviour
    {
        public PuzzleLevel PuzzleLevel;

        public void LevelLoaded()
        {
            PuzzleLevel.LevelLoaded();
        }
    }
}
