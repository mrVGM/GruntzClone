using Base.Actors;
using Base.MessagesSystem;
using Base.Status;
using Gruntz.Equipment;
using System.Linq;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class TriggerShowNotificationActionDef : TriggerBoxActionDef
    {
        public class Notification
        {
            public string NotificationText;
        }

        public MessagesBoxTagDef MessagesBox;
        public StatusDef RegularActorStatus;

        public override void TriggerEnter(Collider ownCollider, Collider otherCollider)
        {
            var ownActorProxy = ownCollider.GetComponent<ActorProxy>();
            var ownActor = ownActorProxy.Actor;

            var otherActorProxy = otherCollider.GetComponent<ActorProxy>();
            if (otherActorProxy == null) {
                return;
            }
            var otherActor = otherActorProxy.Actor;
            var statusComponent = otherActor.GetComponent<StatusComponent>();
            if (statusComponent.GetStatus(RegularActorStatus) == null) {
                return;
            }

            var equipmentComponent = ownActor.GetComponent<EquipmentComponent>();
            var notificationItem = equipmentComponent.Weapon;

            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            messagesSystem.SendMessage(MessagesBox, Base.MainUpdaterUpdateTime.Update, ownActor, new Notification { NotificationText = notificationItem.Description });

            var switchComponent = ownActor.GetComponent<SwitchState.SwitchStateComponent>();
            var stateStatus = switchComponent.SwitchStateComponentDef.StateStatuses.Skip(1).FirstOrDefault();
            switchComponent.SetCurrentState(stateStatus);
        }

        public override void TriggerExit(Collider ownCollider, Collider otherCollider)
        {
            var ownActorProxy = ownCollider.GetComponent<ActorProxy>();
            var ownActor = ownActorProxy.Actor;

            var otherActorProxy = otherCollider.GetComponent<ActorProxy>();
            if (otherActorProxy == null) {
                return;
            }
            var otherActor = otherActorProxy.Actor;
            var statusComponent = otherActor.GetComponent<StatusComponent>();
            if (statusComponent.GetStatus(RegularActorStatus) == null) {
                return;
            }

            var switchComponent = ownActor.GetComponent<SwitchState.SwitchStateComponent>();
            var stateStatus = switchComponent.SwitchStateComponentDef.StateStatuses.FirstOrDefault();
            switchComponent.SetCurrentState(stateStatus);
        }
    }
}
