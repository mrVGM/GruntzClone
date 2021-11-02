using Base;
using System.Linq;
using UnityEngine;

namespace Gruntz.UI
{
    public class Menu : MonoBehaviour
    {
        public GameObject OpenMenu;
        public GameObject ClosedMenu;

        public GameObject QuickLoad;

        public TagDef QuickSave;
        public TagDef LevelProgressSaveTag;

        public void RefreshLoadButton()
        {
            QuickLoad.SetActive(false);
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.LastOrDefault(x => x.SaveTag == QuickSave);
            if (save != null) {
                QuickLoad.SetActive(true);
            }
        }

        public void Toggle()
        {
            OpenMenu.gameObject.SetActive(!OpenMenu.gameObject.activeSelf);
            ClosedMenu.gameObject.SetActive(!OpenMenu.gameObject.activeSelf);
            RefreshLoadButton();
        }

        public void Save()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            savesManager.CreateSave(QuickSave);
            RefreshLoadButton();
        }
        public void LoadLast()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.LastOrDefault(x => x.SaveTag == QuickSave);
            if (save != null)
            {
                savesManager.LoadSave(save, () => { });
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
            if (save != null)
            {
                savesManager.LoadSave(save, () => { });
            }
        }
    }
}
