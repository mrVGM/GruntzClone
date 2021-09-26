using Base.Actors;
using Base.Status;
using Gruntz.Items;
using Gruntz.Statuses;

namespace Gruntz.Equipment
{
    public class EquipmentComponent : IActorComponent
    {
        public EquipmentComponentDef EquipmentComponentDef { get; }
        public Actor Actor { get; }
        public EquipmentComponent(EquipmentComponentDef equipmentComponentDef, Actor actor)
        {
            EquipmentComponentDef = equipmentComponentDef;
            Actor = actor;
        }
        public void DeInit()
        {
        }

        public void Init()
        {
            var statusComponent = Actor.GetComponent<StatusComponent>();
            var weaponStatus = statusComponent.GetStatus(EquipmentComponentDef.WeaponHolderStatus);
            if (weaponStatus == null) {
                weaponStatus = EquipmentComponentDef.WeaponHolderStatus.Data.CreateStatus();
                statusComponent.AddStatus(weaponStatus);
            }
            var auxItemStatus = statusComponent.GetStatus(EquipmentComponentDef.AuxItemHolderStatus);
            if (auxItemStatus == null) {
                auxItemStatus = EquipmentComponentDef.AuxItemHolderStatus.Data.CreateStatus();
                statusComponent.AddStatus(auxItemStatus);
            }
        }

        public ItemDef Weapon
        {
            get 
            {
                var statusComponent = Actor.GetComponent<StatusComponent>();
                var status = statusComponent.GetStatus(EquipmentComponentDef.WeaponHolderStatus);
                var itemHolder = status.StatusData as ItemHolderStatusData;
                return itemHolder.Item;
            }
            set
            {
                var statusComponent = Actor.GetComponent<StatusComponent>();
                var status = statusComponent.GetStatus(EquipmentComponentDef.WeaponHolderStatus);
                var itemHolder = status.StatusData as ItemHolderStatusData;
                itemHolder.Item = value.ToDefRef<ItemDef>();
            }
        }

        public ItemDef SpecialItem
        {
            get
            {
                var statusComponent = Actor.GetComponent<StatusComponent>();
                var status = statusComponent.GetStatus(EquipmentComponentDef.AuxItemHolderStatus);
                var itemHolder = status.StatusData as ItemHolderStatusData;
                return itemHolder.Item;
            }
            set
            {
                var statusComponent = Actor.GetComponent<StatusComponent>();
                var status = statusComponent.GetStatus(EquipmentComponentDef.AuxItemHolderStatus);
                var itemHolder = status.StatusData as ItemHolderStatusData;
                itemHolder.Item = value.ToDefRef<ItemDef>();
            }
        }
    }
}
