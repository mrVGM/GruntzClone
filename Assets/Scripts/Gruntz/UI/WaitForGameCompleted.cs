using System.Collections.Generic;
using System.Linq;
using Base;
using Base.Actors;
using Base.Status;
using Base.UI;
using Gruntz.Equipment;
using Gruntz.Items;
using Gruntz.TriggerBox;

namespace Gruntz.UI
{
    public class WaitForGameCompleted : CoroutineProcess
    {
        public StatusDef RegularActorStatusDef;
        public StatusDef GameCompletedStatusDef;
        public ItemDef ThrophyItem;
        public ProcessContextTagDef NotificationTagDef;
        public TagDef ProgressSaveTagDef;

        public NotificationDataBehaviour.Notification Notification;

        protected override IEnumerator<object> Crt()
        {
            var actorManager = ActorManager.GetActorManagerFromContext();
            var actors = actorManager.Actors.Where(x => {
                var statusComponent = x.GetComponent<StatusComponent>();
                var regularActor = statusComponent.GetStatus(RegularActorStatusDef);
                if (regularActor == null) {
                    return false;
                }

                var equipment = x.GetComponent<EquipmentComponent>();
                if (equipment.Weapon == ThrophyItem) {
                    return true;
                }

                return false;
            });

            while (true) {
                var trophyHolder = actors.FirstOrDefault();
                if (trophyHolder == null) {
                    yield return null;
                    continue;
                }

                var statusComponent = trophyHolder.GetComponent<StatusComponent>();
                var gameCompletedStatus = statusComponent.GetStatus(GameCompletedStatusDef);
                if (gameCompletedStatus != null) {
                    while (true) {
                        yield return null;
                    }
                }

                statusComponent.AddStatus(GameCompletedStatusDef.Data.CreateStatus());
                var game = Game.Instance;
                var savesManager = game.SavesManager;
                savesManager.CreateSave(ProgressSaveTagDef);
                context.PutItem(NotificationTagDef, new[] { Notification });
                break;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
