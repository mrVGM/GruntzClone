using Base;
using Base.Actors;
using Base.Status;
using Gruntz.Equipment;
using Gruntz.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Actors
{
    public class ActorTemplateDef : Def
    {
        public ActorTemplateDef ParentTemplate;
        [SerializeField]
        private ActorDef ActorDef = null;
        [SerializeField]
        private ActorComponentDef[] ActorComponents = null;
        public StatusDef[] Statuses;
        public ItemDef Weapon;
        public ItemDef SpecialItem;

        public IEnumerable<ActorComponentDef> Components
        {
            get
            {
                if (ParentTemplate == null) {
                    foreach (var comp in ActorComponents) {
                        yield return comp;
                    }
                    yield break;
                }

                var used = new List<ActorComponentDef>();
                foreach (var comp in ParentTemplate.Components) {
                    var compType = comp.GetType();
                    var overrideComponent = ActorComponents.FirstOrDefault(x => compType.IsAssignableFrom(x.GetType()));
                    if (overrideComponent != null) {
                        yield return overrideComponent;
                        used.Add(overrideComponent);
                    }
                    else {
                        yield return comp;
                    }
                }

                foreach (var comp in ActorComponents.Except(used)) {
                    yield return comp;
                }
            }
        }

        public ActorDef ActorPrefabDef
        {
            get
            {
                if (ActorDef != null) {
                    return ActorDef;
                }
                if (ParentTemplate != null) {
                    return ParentTemplate.ActorPrefabDef;
                }
                return null;
            }
        }

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
