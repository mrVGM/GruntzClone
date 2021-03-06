using System.Collections.Generic;
using Base.Actors;
using Base.MessagesSystem;
using Base.Status;
using System.Linq;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class TriggerShowNotificationActionDef : TriggerBoxActionDef
    {
        public MessagesBoxTagDef MessagesBox;
        public StatusDef RegularActorStatus;
        public StatusDef NotificationShownStatus;

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

            if (statusComponent.GetStatus(NotificationShownStatus) != null) {
                return;
            }

            statusComponent.AddStatus(NotificationShownStatus.Data.CreateStatus());

            IEnumerable<NotificationDataBehaviour.Notification> notifications = ownActor.ActorComponent.GetComponent<NotificationDataBehaviour>().Notifications;
            
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            messagesSystem.SendMessage(MessagesBox,
                Base.MainUpdaterUpdateTime.Update,
                ownActor,
                notifications);

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
            
            var statuses = statusComponent.GetStatuses(x => x.StatusDef == NotificationShownStatus).ToList();
            foreach (var status in statuses) {
                statusComponent.RemoveStatus(status);
            }

            var switchComponent = ownActor.GetComponent<SwitchState.SwitchStateComponent>();
            var stateStatus = switchComponent.SwitchStateComponentDef.StateStatuses.FirstOrDefault();
            switchComponent.SetCurrentState(stateStatus);
        }
    }
}
