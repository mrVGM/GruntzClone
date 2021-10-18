using Base;
using System.Linq;
using UnityEngine;

namespace Utils.Test
{
    public class SaveGame : MonoBehaviour
    {
        public TagDef QuickSave;
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
    }
}