using Base;
using System.Linq;
using UnityEngine;

namespace Utils.Test
{
    public class SaveGame : MonoBehaviour
    {
        public TagDef QuickSave;
        public TagDef LevelProgressSaveTag;
        public void Save()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            savesManager.CreateSave(QuickSave);
        }
        public void LoadLast()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.LastOrDefault(x => x.SaveTag == QuickSave);
            if (save != null) {
                savesManager.LoadSave(save);
            }
        }

        public void Restart()
        {
            var game = Game.Instance;
            game.LoadLevel(game.currentLevel, () => { });
        }

        public void BackToMenu()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.LastOrDefault(x => x.SaveTag == LevelProgressSaveTag);
            if (save != null) {
                savesManager.LoadSave(save);
            }
        }
    }
}