using Base;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utils.Test
{
    public class SaveGame : MonoBehaviour
    {
        public void Save()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            savesManager.CreateSave();
        }
        public void LoadLast()
        {
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.saves.LastOrDefault();
            if (save != null)
            {
                savesManager.LoadSave(save);
            }
        }
    }
}