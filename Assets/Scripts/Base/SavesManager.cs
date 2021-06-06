using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class SavesManager : MonoBehaviour
    {
        public class Save
        {
            public string SaveName;
            public SavedGame SavedGame;
        }

        public List<Save> saves = new List<Save>();
        public IEnumerable<Save> Saves => saves;

        public void CreateSave()
        {
            var game = Game.Instance;
            var savedGame = new SavedGame
            {
                Level = game.currentLevel,
                SerializedContextObjects = game.Context.GetSerializedContextObjects().ToList(),
            };
            var save = new Save
            {
                SaveName = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                SavedGame = savedGame
            };
            saves.Add(save);
        }

        public void LoadSave(Save save)
        {
            var game = Game.Instance;
            game.LoadLevel(save.SavedGame.Level, () => {
                var savedGameHolder = SavedGameHolder.GetSavedGameHolderFromGame();
                savedGameHolder.SavedGame = save.SavedGame;
            });
        }
    }
}
