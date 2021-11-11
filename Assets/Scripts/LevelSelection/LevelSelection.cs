using Base;
using LevelResults;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Base.MainUpdaterLock;

namespace LevelSelection
{
    public class LevelSelection : MonoBehaviour
    {
        public Transform BridgesContainer;
        public Transform SitesContainer;
        public LevelSelectionMapUnit Unit;
        public TagDef FinishedLevelsSaveTagDef;

        public IEnumerable<BezierLine> Bridges => BridgesContainer.GetComponentsInChildren<BezierLine>();
        public IEnumerable<Site> Sites => SitesContainer.GetComponentsInChildren<Site>();

        private List<ILock> _locks;
        public void LevelLoaded()
        {
            var game = Game.Instance;

            _locks = new List<ILock>();
            var orderTagDef = game.DefRepositoryDef.AllDefs.OfType<LevelLoadedOrderTagDef>().First();
            foreach (var tag in game.MainUpdater.ExecutionOrder) {
                if (tag == orderTagDef)
                {
                    continue;
                }
                var l = game.MainUpdater.MainUpdaterLock.TryLock(tag);
                _locks.Add(l);
            }

            Init();

            foreach (var l in _locks) {
                game.MainUpdater.MainUpdaterLock.Unlock(l);
            }
        }

        private void Init()
        {
            var savedGameHolder = SavedGameHolder.GetSavedGameHolderFromContext();
            if (savedGameHolder.SavedGame != null) {
                savedGameHolder.RestoreContext();
            }

            var levelProgress = LevelProgressInfo.GetLevelProgressInfoFromContext();

            var levelProgressInfoData = levelProgress.Data as LevelProgressInfoData;

            var levelSelectionMap = LevelSelectionMap.GetLevelSelectionMapFromContext();

            var levelResult = LevelResultHolder.GetLevelResultHolderFromContext();
            if (levelResult.LevelResult != null) {
                var result = (PuzzleLevelResult.Result)levelResult.LevelResult;
                var game = Game.Instance;
                if (result == PuzzleLevelResult.Result.Completed) {
                    var completedSoFar = levelProgressInfoData.FinishedLevels.Select(x => (LevelDef)x);
                    if (!completedSoFar.Contains(levelResult.Level)) {
                        levelProgressInfoData.FinishedLevels = completedSoFar.Append(levelResult.Level)
                            .Select(x => x.ToDefRef<LevelDef>()).ToList();
                        game.SavesManager.CreateSave(FinishedLevelsSaveTagDef);
                    }
                }
            }

            var initialSite = Sites.FirstOrDefault(x => x.LevelDef == levelProgress.CurrentLevel);
            levelSelectionMap.InitMap(Sites, Bridges, Unit, initialSite);
        }

        private void InitBridges()
        {
        }
    }
}
