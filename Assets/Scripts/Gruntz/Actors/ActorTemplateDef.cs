using Base;
using Base.Actors;
using Base.Status;
using Gruntz.Equipment;
using Gruntz.Items;

namespace Gruntz.Actors
{
    public class ActorTemplateDef : Def
    {
        public ActorDef ActorDef;
        public ActorComponentDef[] ActorComponents;
        public StatusDef[] Statuses;
        public ItemDef Weapon;
        public ItemDef SpecialItem;

        public void ProcessActor(Actor actor)
        {
            var statusComponent = actor.GetComponent<StatusComponent>();
            if (statusComponent != null) {
                foreach (var statusDef in Statuses) {
                    statusComponent.AddStatus(statusDef.Data.CreateStatus());
                }
            }
            var equipment = actor.GetComponent<EquipmentComponent>();
            if (equipment != null) {
                if (Weapon != null) {
                    equipment.Weapon = Weapon;
                }
                if (SpecialItem != null) {
                    equipment.SpecialItem = SpecialItem;
                }
            }
        }
    }
}
