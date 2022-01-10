using Base.Actors;
using Gruntz.Equipment;
using Gruntz.Items;
using Gruntz.Team;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class TriggerGiveItemActionDef : TriggerBoxActionDef
    {
        public override void TriggerEnter(Collider ownCollider, Collider otherCollider)
        {
            var ownActorProxy = ownCollider.GetComponent<ActorProxy>();
            var ownActor = ownActorProxy.Actor;

            var ownEquipment = ownActor.GetComponent<EquipmentComponent>();

            var actorProxy = otherCollider.GetComponent<ActorProxy>();
            if (actorProxy == null) {
                return;
            }
            var actor = actorProxy.Actor;

            var teamComponent = actor.GetComponent<TeamComponent>();
            if (teamComponent == null) {
                return;
            }

            if (teamComponent.UnitTeam != TeamComponent.Team.Player) {
                return;
            }

            var equipmentComponent = actor.GetComponent<EquipmentComponent>();
            if (equipmentComponent == null) {
                return;
            }
            if (ownEquipment.Weapon != null) {
                equipmentComponent.Weapon = ownEquipment.Weapon;
            }
            if (ownEquipment.SpecialItem != null) {
                equipmentComponent.SpecialItem = ownEquipment.SpecialItem;
            }

            ownActor.Deinit();
        }

        public override void TriggerExit(Collider ownCollider, Collider otherCollider)
        {
        }
    }
}
