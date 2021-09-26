using Base.Actors;
using Gruntz.Statuses;

namespace Gruntz.Equipment
{
    public class EquipmentComponentDef : ActorComponentDef
    {
        public ItemHolderStatusDef WeaponHolderStatus;
        public ItemHolderStatusDef AuxItemHolderStatus;

        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new EquipmentComponent(this, actor);
        }
    }
}
