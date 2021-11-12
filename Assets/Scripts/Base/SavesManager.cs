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
#if UNITY_WEBGL
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void SyncDB();
#endif

        [Serializable]
        public class Save
        {
            public string SaveName;
            public DefRef<TagDef> Tag;
            public TagDef SaveTag => Tag;
            public byte[] SavedGame;
        }

        private List<Save> _saves = new List<Save>();
        public IEnumerable<Save> Saves => _saves;
        private bool _flushRequested = false;

        public void CreateSave(TagDef tag)
        {
            _saves.RemoveAll(x => x.SaveTag == tag);
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
                    SaveName = DateTime.Now.ToString($"{tag.name}.gruntzsave"),
                    Tag = tag.ToDefRef<TagDef>(),
                    SavedGame = memSteam.GetBuffer(),
                };
                _saves.Add(save);
            }
            _flushRequested = true;
        }

        public void LoadSave(Save save, Action onLoaded)
        {
            var game = Game.Instance;
            var binaryFormatter = new BinaryFormatter();
            using (var memStream = new MemoryStream(save.SavedGame))
            {
                var savedGame = binaryFormatter.Deserialize(memStream) as SavedGame;
                game.LoadLevel(savedGame.Level, () =>
                {
                    var savedGameHolder = SavedGameHolder.GetSavedGameHolderFromContext();
                    savedGameHolder.SavedGame = savedGame;
                    onLoaded();
                });
            }
        }

        public void RetrieveSaves()
        {
            var files = Directory.GetFiles(Application.persistentDataPath).Where(x => x.EndsWith(".gruntzsave"));
            foreach (var path in files) {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    var binaryFormatter = new BinaryFormatter();
                    var save = binaryFormatter.Deserialize(fileStream) as Save;
                    _saves.Add(save);
                }
            }
        }

        private IEnumerator<object> FlushSavesCrt()
        {
            string persistentDataPath = Application.persistentDataPath;
            while (true) {
                while (!_flushRequested) {
                    yield return null;
                }
                _flushRequested = false;

                int index = 0;
                foreach (var save in Saves) {
                    var path = Path.Combine(persistentDataPath, $"{index++}.gruntzsave");
                    using (var fileStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write)) {
                        var binaryFormatter = new BinaryFormatter();
                        binaryFormatter.Serialize(fileStream, save);
                    }
                }
#if UNITY_WEBGL
                SyncDB();
#endif
            }
        }
        private void Awake()
        {
            StartCoroutine(FlushSavesCrt());
        }
    }
}
