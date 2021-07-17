using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Base
{
    [Serializable]
    public class SavesManager : MonoBehaviour
    {
        public class Save
        {
            public string SaveName;
            public byte[] SavedGame;
        }

        public List<Save> saves = new List<Save>();
        public IEnumerable<Save> Saves => saves;

        public void CreateSave()
        {
            var game = Game.Instance;
            var savedGame = new SavedGame
            {
                Level = game.currentLevel.ToDefRef<LevelDef>(),
                SerializedContextObjects = game.Context.GetSerializedContextObjects().ToList(),
            };

            var binaryFormatter = new BinaryFormatter();
            using (var memSteam = new MemoryStream()) {
                binaryFormatter.Serialize(memSteam, savedGame);
                var save = new Save
                {
                    SaveName = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                    SavedGame = memSteam.GetBuffer(),
                };
                saves.Add(save);
            }
        }

        public void LoadSave(Save save)
        {
            var game = Game.Instance;
            var binaryFormatter = new BinaryFormatter();
            using (var memStream = new MemoryStream(save.SavedGame))
            {
                var savedGame = binaryFormatter.Deserialize(memStream) as SavedGame;
                game.LoadLevel(savedGame.Level, () =>
                {
                    var savedGameHolder = SavedGameHolder.GetSavedGameHolderFromGame();
                    savedGameHolder.SavedGame = savedGame;
                });
            }
        }
    }
}
