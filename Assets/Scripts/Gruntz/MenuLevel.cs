using Base;
using System.Linq;
using UnityEngine;

namespace Gruntz
{
    public class MenuLevel : MonoBehaviour
    {
        public LevelDef PuzzleLevelDef;
        public TagDef LevelProgress;

        public void LoadPuzzleLevel()
        {
            var game = Game.Instance;
            var save = game.SavesManager.Saves.FirstOrDefault(x => x.Tag == LevelProgress);

            if (save == null) {
                game.LoadLevel(PuzzleLevelDef, () => { });
                return;
            }

            game.SavesManager.LoadSave(save, () => { });
        }
    }
}
