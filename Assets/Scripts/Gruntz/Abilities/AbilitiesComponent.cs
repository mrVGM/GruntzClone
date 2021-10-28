using Base;
using Base.Actors;
using Base.Status;
using Gruntz.Equipment;
using Gruntz.Items;
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

        public AbilityPlayer Current { get; private set; }
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

        public IEnumerable<AbilityDef> GetAbilities()
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

        public ItemDef GetSourceItem(AbilityDef ability)
        {
            var equipmentComponent = Actor.GetComponent<EquipmentComponent>();
            if (equipmentComponent.Weapon.Abilities.Contains(ability)) {
                return equipmentComponent.Weapon;
            }

            if (equipmentComponent.SpecialItem == null) {
                return null;
            }
            if (equipmentComponent.SpecialItem.Abilities.Contains(ability)) {
                return equipmentComponent.SpecialItem;
            }
            return null;
        }

        public bool IsOnCooldown(AbilityDef ability)
        {
            var downtime = GetAbilityDownTime(ability);
            return downtime <= ability.Cooldown;
        }

        public bool IsEnabled(AbilityDef ability)
        {
            var abilityManager = AbilityManager.GetAbilityManagerFromContext();
            bool playing = abilityManager.AbilityPlayers
                .Any(x => x.AbilityDef == ability && x.Actor == Actor && x.State.GeneralState == AbilityPlayer.GeneralExecutionState.Playing);
            if (playing) {
                return false;
            }

            return !IsOnCooldown(ability);
        }

        public bool CanExecuteOn(AbilityDef ability, Actor actor)
        {
            var statusComponent = actor.GetComponent<StatusComponent>();
            if (statusComponent == null) {
                return false;
            }

            if (!statusComponent.GetStatuses(x => ability.TargetActorStatuses.Contains(x.StatusDef)).Any()) {
                return false;
            }

            var abilityManager = AbilityManager.GetAbilityManagerFromContext();
            var otherPlayers = abilityManager.AbilityPlayers.Where(x => x.AbilityDef == ability);
            otherPlayers = otherPlayers.Where(x => x.Target == actor);
            if (otherPlayers.Any(x => x.Actor != Actor)) {
                return false;
            }
            return true;
        }

        public void ActivateAbility(AbilityDef ability, object target)
        {
            var abilitiesManager = AbilityManager.GetAbilityManagerFromContext();
            var executionContext = new AbilityDef.AbilityExecutionContext {
                Actor = Actor,
                Target = target,
                OnFinished = () => {
                    if (Current.State.CooldownState == AbilityPlayer.CooldownState.NoCooldown) {
                        return;
                    }
                    var record = _abilitiesComponentData.AbilitiesUsage.FirstOrDefault(x => x.Ability == ability);
                    if (record == null) {
                        record = new AbilitiesComponentData.AbilityUsageRecord {
                            Ability = ability.ToDefRef<AbilityDef>()
                        };
                        _abilitiesComponentData.AbilitiesUsage.Add(record);
                    }
                    record.Downtime = 0;
                    record.LastUsage = Time.time;
                }
            };

            var player = new AbilityPlayer(ability, executionContext);
            if (Current != null) {
                Current.Interrupt();
            }
            Current = player;
            abilitiesManager.AbilityPlayers.Add(Current);
        }

        public AbilityDef GetMainAbility()
        {
            return GetAbilities().FirstOrDefault();
        }

        public AbilityDef GetAttackAbility()
        {
            return GetAbilities().FirstOrDefault(x => x is IAttackAbility);
        }

        public float GetAbilityDownTime(AbilityDef ability)
        {
            var abilityItem = GetSourceItem(ability);
            var records = _abilitiesComponentData.AbilitiesUsage.Where(x => GetSourceItem(x.Ability) == abilityItem);

            if (!records.Any()) {
                return float.PositiveInfinity;
            }

            return Time.time - records.Max(x => x.LastUsage);
        }

        public void DeInit()
        {
            if (Current != null) {
                Current.Interrupt();
            }
        }

        public void Init()
        {
        }
    }
}
