using System.Linq;
using Base;
using Base.Actors;
using Base.Status;
using Gruntz.SwitchState;
using LevelResults;
using UnityEngine;

namespace Gruntz
{
    public class PuzzleLevelSelection : MonoBehaviour
    {
        public ActorProxy Bridge;
        public ActorProxy Bridge2;
        public ActorProxy PistonsSwitch;
        public LevelDef[] BridgeUnlockReqs;
        public LevelDef[] PistonsUnlockReqs;
        public LevelDef[] Bridge2UnlockReqs;

        public StatusDef BridgeUp;
        public StatusDef BridgeDown;
        public StatusDef PistonsUp;
        public StatusDef PistonsDown;
        
        public PuzzleLevel PuzzleLevel;
        public TagDef ProgressSaveTagDef;

        public void LevelLoaded()
        {
            PuzzleLevel.LevelLoaded();
            
            var levelResultsHolder = LevelResultHolder.GetLevelResultHolderFromContext();
            var levelProgress = LevelProgressInfo.GetLevelProgressInfoFromContext();
            var levelProgressInfoData = levelProgress.Data as LevelProgressInfoData;
            var finished = levelProgressInfoData.FinishedLevels;
            if (levelResultsHolder.LevelResult != null && (PuzzleLevelResult.Result)levelResultsHolder.LevelResult == PuzzleLevelResult.Result.Completed) {
                if (!finished.Select(x => (LevelDef)x).Contains(levelResultsHolder.Level)) {
                    finished.Add(levelResultsHolder.Level.ToDefRef<LevelDef>());
                }
            }

            var finishedLevelDefs = finished.Select(x => (LevelDef)x);
            var bridgeActor = Bridge.Actor;
            var bridge2Actor = Bridge.Actor;
            var pistonsSwitchActor = PistonsSwitch.Actor;

            var bridgeSwitchComponent = bridgeActor.GetComponent<SwitchStateComponent>();
            var bridge2SwitchComponent = bridge2Actor.GetComponent<SwitchStateComponent>();
            var pistonsSwitchComponent = pistonsSwitchActor.GetComponent<SwitchStateComponent>();
            
            if (finishedLevelDefs.Intersect(BridgeUnlockReqs).Count() == BridgeUnlockReqs.Length) {
                bridgeSwitchComponent.SetCurrentState(BridgeUp);
            }
            else {
                bridgeSwitchComponent.SetCurrentState(BridgeDown);
            }
            
            if (finishedLevelDefs.Intersect(PistonsUnlockReqs).Count() == PistonsUnlockReqs.Length) {
                pistonsSwitchComponent.SetCurrentState(PistonsDown);
            }
            else {
                pistonsSwitchComponent.SetCurrentState(PistonsUp);
            }
            
            if (finishedLevelDefs.Intersect(Bridge2UnlockReqs).Count() == Bridge2UnlockReqs.Length) {
                bridge2SwitchComponent.SetCurrentState(BridgeUp);
            }
            else {
                bridge2SwitchComponent.SetCurrentState(BridgeDown);
            }

            var game = Game.Instance;
            var savesManager = game.SavesManager;
            savesManager.CreateSave(ProgressSaveTagDef);
        }
    }
}
