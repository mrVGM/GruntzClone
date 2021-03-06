using Base.Actors;
using Base.Status;
using Gruntz.EquipmentVisuals;
using Gruntz.Items;
using Gruntz.Statuses;
using System.Linq;
using UnityEngine;

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
            var item = (ItemDef)(weaponStatus.Data as ItemHolderStatusData).Item;
            if (item != null) {
                Weapon = item;
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

                var equipmentRoot = Actor.ActorComponent.GetComponentInChildren<EquipmentRoot>(true);
                if (equipmentRoot == null) {
                    return;
                }
                while (equipmentRoot.transform.childCount > 0) {
                    var child = equipmentRoot.transform.GetChild(0);
                    child.SetParent(null);
                    GameObject.Destroy(child.gameObject);
                }
                if (value.Prefab != null) {
                    var go = GameObject.Instantiate(value.Prefab, equipmentRoot.transform);
                    var laggingPoints = go.GetComponentsInChildren<LaggingPoint>(true);
                    var laggingPointTargets = Actor.ActorComponent.GetComponentsInChildren<LaggingPointTarget>(true);
                    foreach (var laggingPoint in laggingPoints) {
                        laggingPoint.Target = laggingPointTargets.FirstOrDefault(x => x.TagDef == laggingPoint.Tag);
                        laggingPoint.transform.position = laggingPoint.Target.transform.position;
                    }
                }
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

        public void EnableLagging(bool enabled)
        {
            var equipmentRoot = Actor.ActorComponent.GetComponentInChildren<EquipmentRoot>();
            var laggingPoints = equipmentRoot.GetComponentsInChildren<LaggingPoint>(true);

            foreach (var point in laggingPoints) {
                if (point.Target == null) {
                    continue;
                }
                point.Target.LaggingEnabled = enabled;
            }
        }
    }
}
