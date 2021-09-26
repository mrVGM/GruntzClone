using Base.Actors;
using Gruntz.Equipment;
using Gruntz.Items;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class TriggerGiveItemActionDef : TriggerBoxActionDef
    {
        public ItemDef Weapon;
        public ItemDef SpecialItem;
        public override void TriggerEnter(Collider ownCollider, Collider otherCollider)
        {
            var ownActorProxy = ownCollider.GetComponent<ActorProxy>();
            var ownActor = ownActorProxy.Actor;

            var actorProxy = otherCollider.GetComponent<ActorProxy>();
            if (actorProxy == null) {
                return;
            }
            var actor = actorProxy.Actor;
            var equipmentComponent = actor.GetComponent<EquipmentComponent>();
            if (equipmentComponent == null) {
                return;
            }
            if (Weapon != null) {
                equipmentComponent.Weapon = Weapon;
            }
            if (SpecialItem != null) {
                equipmentComponent.SpecialItem = SpecialItem;
            }

            ownActor.Deinit();
        }

        public override void TriggerExit(Collider ownCollider, Collider otherCollider)
        {
        }
    }
}
