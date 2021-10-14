using Base;
using Base.Actors;
using Gruntz.Equipment;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class AbilitiesComponent : IActorComponent, ISerializedObject
    {
        public AbilitiesComponentDef AbilitiesComponentDef { get; }
        public Actor Actor { get; }

        private AbilitiesComponentData _abilitiesComponentData;
        public ISerializedObjectData Data
        {
            get
            {
                if (_abilitiesComponentData == null) {
                    _abilitiesComponentData = new AbilitiesComponentData();
                }
                foreach (var abilityRecord in _abilitiesComponentData.AbilitiesUsage) {
                    abilityRecord.Downtime = Time.time - abilityRecord.LastUsage;
                }
                return _abilitiesComponentData;
            }
            set
            {
                _abilitiesComponentData = value as AbilitiesComponentData;
                foreach (var record in _abilitiesComponentData.AbilitiesUsage) {
                    record.LastUsage = Time.time - record.Downtime;
                }
            }
        }

        public AbilitiesComponent(Actor actor, AbilitiesComponentDef abilitiesComponentDef)
        {
            Actor = actor;
            AbilitiesComponentDef = abilitiesComponentDef;
        }

        public IEnumerable<HitAbilityDef> GetAbilities()
        {
            var equipmentComponent = Actor.GetComponent<EquipmentComponent>();
            foreach (var ability in equipmentComponent.Weapon.Abilities) {
                yield return ability;
            }
            if (equipmentComponent.SpecialItem == null) {
                yield break;
            }
            foreach (var ability in equipmentComponent.SpecialItem.Abilities) {
                yield return ability;
            }
        }

        public bool IsEnabled(HitAbilityDef ability)
        {
            var downtime = GetAbilityDownTime(ability);
            return downtime > ability.Cooldown;
        }
        public void ActivateAbility(HitAbilityDef ability, object target)
        {
            var abilitiesManager = AbilityManager.GetAbilityManagerFromContext();
            abilitiesManager.AbilityPlayers.Add(new AbilityPlayer(ability, Actor, target));
            var record = _abilitiesComponentData.AbilitiesUsage.FirstOrDefault(x => x.Ability == ability);
            if (record == null) {
                record = new AbilitiesComponentData.AbilityUsageRecord {
                    Ability = ability.ToDefRef<HitAbilityDef>()
                };
                _abilitiesComponentData.AbilitiesUsage.Add(record);
            }
            record.Downtime = 0;
            record.LastUsage = Time.time;
        }

        public HitAbilityDef GetMainAbility()
        {
            return GetAbilities().FirstOrDefault();
        }

        public float GetAbilityDownTime(HitAbilityDef ability)
        {
            var record = _abilitiesComponentData.AbilitiesUsage.FirstOrDefault(x => x.Ability == ability);
            if (record == null) {
                return float.PositiveInfinity;
            }

            return Time.time - record.LastUsage;
        }

        public void DeInit()
        {
        }

        public void Init()
        {
        }
    }
}
