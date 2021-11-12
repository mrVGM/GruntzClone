using Base;
using System.Linq;
using UnityEngine;

namespace Gruntz
{
    public class MenuLevel : MonoBehaviour
    {
        public LevelDef LevelSelectionDef;
        public TagDef LevelProgress;

        public void LoadLevelSelection()
        {
            var game = Game.Instance;
            var save = game.SavesManager.Saves.FirstOrDefault(x => x.Tag == LevelProgress);

            if (save == null) {
                game.LoadLevel(LevelSelectionDef, () => { });
                return;
            }

            game.SavesManager.LoadSave(save, () => { });
        }
    }
}
