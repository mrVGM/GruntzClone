using Base;
using Gruntz.LevelProgress;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Gruntz
{
    public class LevelSelectionLevel : MonoBehaviour
    {
        public RectTransform ButtonsContainer;
        public Button SelectionButton;
        public LevelProgressInfoDef LevelProgressInfoDef;
        public TagDef FinishedLevelsSaveTagDef;

        public void InitButtons()
        {
            var game = Game.Instance;
            var savedGameHolder = SavedGameHolder.GetSavedGameHolderFromContext();
            if (savedGameHolder.SavedGame != null) {
                foreach (var pair in savedGameHolder.SavedGame.SerializedContextObjects) {
                    var contextObject = game.Context.GetRuntimeObject(pair.Def);
                    var serializedObject = contextObject as ISerializedObject;
                    serializedObject.Data = pair.ContextObjectData;
                }
            }

            var levelProgressInfo = game.Context.GetRuntimeObject(LevelProgressInfoDef) as LevelProgressInfo;
            var levelProgresInfoData = levelProgressInfo.Data as LevelProgressInfoData;

            for (int i = 0; i < ButtonsContainer.childCount; ++i) {
                var child = ButtonsContainer.GetChild(i);
                child.gameObject.SetActive(false);
            }

            IEnumerable<LevelDef> levelsToDisplay()
            {
                foreach (var level in levelProgresInfoData.FinishedLevels) {
                    yield return level;
                }
            }

            var allLevels = levelsToDisplay();
            var nextLevel = LevelProgressInfoDef.AllLevels.Except(allLevels).FirstOrDefault();

            if (nextLevel != null) {
                allLevels = allLevels.Append(nextLevel);
            }

            int index = 0;
            foreach (var level in allLevels) {
                if (index >= ButtonsContainer.childCount) {
                    while (ButtonsContainer.childCount <= index) {
                        Instantiate(SelectionButton, ButtonsContainer);
                    }
                }

                var button = ButtonsContainer.GetChild(index).GetComponent<Button>();
                button.gameObject.SetActive(true);
                var text = button.GetComponentInChildren<Text>();
                text.text = level.Name;
                button.onClick.AddListener(() => {
                    game.SavesManager.CreateSave(FinishedLevelsSaveTagDef);
                    game.LoadLevel(level, () => { });
                });
                ++index;
            }
        }
    }
}
