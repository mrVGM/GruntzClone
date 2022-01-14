using System.Collections.Generic;
using Base;
using System.Linq;
using UnityEngine;

namespace Gruntz.UI
{
    public class Menu : MonoBehaviour
    {
        public GameObject QuickLoad;
        public TagDef QuickSave;
        public TagDef LevelProgressSaveTag;

        private bool _open = false;
        private List<MainUpdaterLock.ILock> _locks = new List<MainUpdaterLock.ILock>();

        private void RefreshLoadButton()
        {
            QuickLoad.SetActive(false);
            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.LastOrDefault(x => x.SaveTag == QuickSave);
            if (save != null) {
                QuickLoad.SetActive(true);
            }
        }

        private void Pause()
        {
            _locks.Clear();
            var game = Game.Instance;
            var mainUpdater = game.MainUpdater;
            foreach (var tag in mainUpdater.ExecutionOrder) {
                var l = mainUpdater.MainUpdaterLock.TryLock(tag);
                if (l != null) {
                    _locks.Add(l);
                }
            }
        }

        private void Unpause()
        {
            var game = Game.Instance;
            var mainUpdater = game.MainUpdater;
            foreach (var l in _locks) {
                mainUpdater.MainUpdaterLock.Unlock(l);
            }
            _locks.Clear();
        }

        private void Toggle()
        {
            _open = !_open;
            RefreshLoadButton();
            if (_open) {
                Pause();
            }
            else {
                Unpause();
            }

            var animator = GetComponent<Animator>();
            animator.SetBool("Open", _open);
        }

        public void Save()
        {
            if (!_open) {
                return;
            }

            var game = Game.Instance;
            var savesManager = game.SavesManager;
            savesManager.CreateSave(QuickSave);
            RefreshLoadButton();
        }
        public void LoadLast()
        {
            if (!_open) {
                return;
            }
            Unpause();

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
            if (!_open) {
                return;
            }
            Unpause();

            var game = Game.Instance;
            game.LoadLevel(game.currentLevel, () => { });
        }

        public void BackToMenu()
        {
            if (!_open) {
                return;
            }
            Unpause();

            var game = Game.Instance;
            var savesManager = game.SavesManager;
            var save = savesManager.Saves.LastOrDefault(x => x.SaveTag == LevelProgressSaveTag);
            if (save != null) {
                savesManager.LoadSave(save, () => { });
            }
        }

        public void OpenMenu()
        {
            if (_open) {
                return;
            }
            Toggle();
        }

        public void CloseMenu()
        {
            if (!_open) {
                return;
            }
            Toggle();
        }
    }
}
