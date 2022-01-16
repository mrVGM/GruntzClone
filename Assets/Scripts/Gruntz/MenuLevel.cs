using System;
using Base;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gruntz
{
    public class MenuLevel : MonoBehaviour
    {
        public LevelDef LevelSelectionDef;
        public TagDef LevelProgress;

        public Button Continue;
        public Button NewGame;

        private void Start()
        {
            NewGame.gameObject.SetActive(true);
            
            var game = Game.Instance;
            var save = game.SavesManager.Saves.FirstOrDefault(x => x.Tag == LevelProgress);

            if (save != null) {
                Continue.gameObject.SetActive(true);
            }
        }

        public void LoadLevelSelection()
        {
            var game = Game.Instance;
            var save = game.SavesManager.Saves.FirstOrDefault(x => x.Tag == LevelProgress);
            game.SavesManager.LoadSave(save, () => { });
        }

        public void StartNewGame()
        {
            var game = Game.Instance;
            game.LoadLevel(LevelSelectionDef, () => { });
        }
    }
}
