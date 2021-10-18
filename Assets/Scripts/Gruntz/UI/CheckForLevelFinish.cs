using Base;
using Base.Actors;
using Base.Status;
using System.Collections.Generic;
using Base.UI;
using Gruntz.Items;
using System.Linq;
using Gruntz.Equipment;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Gruntz.LevelProgress;

namespace Gruntz.UI
{
    public class CheckForLevelFinish : CoroutineProcess
    {
        public ItemDef TrophyItem;
        public StatusDef RegularActor;
        public TagDef LevelProgressSaveTag;
        public LevelProgressInfoDef LevelProgressInfoDef;
        protected override IEnumerator<object> Crt()
        {
            var actorManager = ActorManager.GetActorManagerFromContext();

            while (true) {
                var activeActors = actorManager.Actors.Where(x => {
                    var statusComponent = x.GetComponent<StatusComponent>();
                    return statusComponent.GetStatus(RegularActor) != null;
                });

                if (!activeActors.Any()) {
                    yield break;
                }

                if (activeActors.Any(x => {
                    var equipmentComponent = x.GetComponent<EquipmentComponent>();
                    if (equipmentComponent == null)
                    {
                        return false;
                    }
                    return equipmentComponent.Weapon == TrophyItem;
                })) {
                    var game = Game.Instance;
                    var savesManager = game.SavesManager;
                    var progressSave = savesManager.Saves.FirstOrDefault(x => x.SaveTag = LevelProgressSaveTag);

                    var binaryFormatter = new BinaryFormatter();
                    SavedGame savedGame = null;
                    using (var memStream = new MemoryStream(progressSave.SavedGame)) {
                        savedGame = binaryFormatter.Deserialize(memStream) as SavedGame;
                    }
                    var levelProgressInfoData = savedGame.SerializedContextObjects
                            .FirstOrDefault(x => x.Def == LevelProgressInfoDef)
                            .ContextObjectData as LevelProgressInfoData;

                    levelProgressInfoData.FinishedLevels.Add(game.currentLevel.ToDefRef<LevelDef>());
                    using (var memStream = new MemoryStream()) {
                        binaryFormatter.Serialize(memStream, savedGame);
                        progressSave.SavedGame = memStream.GetBuffer();
                    }

                    yield break;
                }

                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
