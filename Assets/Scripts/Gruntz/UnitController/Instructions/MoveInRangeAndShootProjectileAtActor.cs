using Base;
using Base.Actors;
using Base.Navigation;
using Gruntz.Abilities;
using Gruntz.Equipment;
using Gruntz.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.UnitController.Instructions
{
    public class MoveInRangeAndShootProjectileAtActor : IUnitExecutable
    {
        [Serializable]
        public struct InRangeTarget : INavigationTarget
        {
            public Map Map => Navigation.GetNavigationFromContext().Map;
            public SerializedVector3 Pos;
            public float Range;
            public Vector3 AdjustPosition(Vector3 pos)
            {
                var snapped = Map.SnapPosition(pos);
                return snapped;
            }

            public bool HasArrived(Vector3 pos)
            {
                var snapped = Map.SnapPosition(pos);
                Vector3 offset = snapped - pos;
                if (offset.sqrMagnitude >= Navigation.Eps) {
                    return false;
                }
                return (Pos - snapped).magnitude <= Range;
            }

            public float Proximity(Vector3 pos)
            {
                return (Pos - pos).sqrMagnitude;
            }
        }

        private Actor _targetActor;
        private ProjectileAttackAbilityDef _abilityDef;

        public MoveInRangeAndShootProjectileAtActor(Actor targetActor, ProjectileAttackAbilityDef ability)
        {
            _targetActor = targetActor;
            _abilityDef = ability;
        }

        private class UpdatingExecute : IUpdatingExecutable
        {
            private enum CrtState
            {
                Active,
                Finished,
                Interrupted
            }

            private enum AbilityPresence
            {
                Present,
                Missing,
                Finished,
            }

            private enum MainCrtState
            {
                Moving,
                WaitingForAbilityEnabled,
                WaitingForAbilityExecuting,
                Finished,
            }

            private IEnumerator<CrtState> _crt;
            private bool _stopped = false;

            public UpdatingExecute(Actor actor, Actor targetActor, ProjectileAttackAbilityDef projectileAbility)
            {
                IEnumerator<AbilityPresence> abilityPresenceCrt()
                {
                    var main = mainCrt();
                    while (true) {
                        var equipment = actor.GetComponent<EquipmentComponent>();
                        var weapon = equipment.Weapon;
                        if (weapon == null) {
                            yield return AbilityPresence.Missing;
                            break;
                        }

                        var ability = weapon.Abilities.OfType<ProjectileAttackAbilityDef>().FirstOrDefault();
                        if (ability != projectileAbility) {
                            yield return AbilityPresence.Missing;
                            break;
                        }
                        main.MoveNext();
                        if (main.Current == MainCrtState.Finished) {
                            yield return AbilityPresence.Finished;
                            break;
                        }
                        yield return AbilityPresence.Present;
                    }
                }

                var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
                var inRangeTarget = new InRangeTarget();
                var projectile = projectileAbility.Projectile.Components.OfType<ProjectileComponentDef>().FirstOrDefault();
                inRangeTarget.Range = projectile.ParabolaSettings.MaxDist;
                bool targetInRange()
                {
                    inRangeTarget.Pos = targetActor.Pos;
                    return inRangeTarget.HasArrived(actor.Pos);
                }

                bool abilityEnabled()
                {
                    return abilitiesComponent.IsEnabled(projectileAbility);
                }

                IEnumerator<MainCrtState> mainCrt() 
                {
                    var navAgent = actor.GetComponent<NavAgent>();
                    navAgent.Target = inRangeTarget;

                    while (true) {
                        if (!targetActor.IsInPlay) {
                            yield return MainCrtState.Finished;
                        }

                        if (!targetInRange()) {
                            yield return MainCrtState.Moving;
                            continue;
                        }

                        if (!abilityEnabled()) {
                            yield return MainCrtState.WaitingForAbilityEnabled;
                            continue;
                        }

                        abilitiesComponent.ActivateAbility(projectileAbility, targetActor.Pos);
                        while (abilitiesComponent.Current != null
                        && abilitiesComponent.Current.State.GeneralState == AbilityPlayer.GeneralExecutionState.Playing) {
                            yield return MainCrtState.WaitingForAbilityExecuting;
                        }
                    }
                }

                IEnumerator<CrtState> stoppableCrt()
                {
                    var presenceCrt = abilityPresenceCrt();
                    while (true) {
                        if (_stopped) {
                            yield return CrtState.Interrupted;
                            break;
                        }

                        presenceCrt.MoveNext();
                        if (presenceCrt.Current == AbilityPresence.Missing || presenceCrt.Current == AbilityPresence.Finished) {
                            yield return CrtState.Finished;
                            break;
                        }
                        yield return CrtState.Active;
                    }
                }
                _crt = stoppableCrt();
            }

            public void StopExecution()
            {
                _stopped = true;
                while (UpdateExecutable()) { }
            }

            public bool UpdateExecutable()
            {
                _crt.MoveNext();
                return _crt.Current == CrtState.Active;
            }
        }

        public IUpdatingExecutable Execute(Actor actor)
        {
            return new UpdatingExecute(actor, _targetActor, _abilityDef);
        }
    }
}
