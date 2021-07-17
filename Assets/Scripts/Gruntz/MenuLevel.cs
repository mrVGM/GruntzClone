using Base;
using UnityEngine;

namespace Gruntz
{
    public class MenuLevel : MonoBehaviour
    {
        public LevelDef PuzzleLevelDef;
        public void LoadPuzzleLevel()
        {
            var game = Game.Instance;
            game.LoadLevel(PuzzleLevelDef, () => { });
        }
    }
}
