using Base.Actors;
using Base.Navigation;
using Gruntz.Abilities;
using Gruntz.Equipment;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.UnitController.Instructions
{
    public class StopAndShootProjectile : IUnitExecutable
    {
        private Vector3 _targetPosition;
        private ProjectileAttackAbilityDef _abilityDef;

        public StopAndShootProjectile(Vector3 targetPosition, ProjectileAttackAbilityDef ability)
        {
            _targetPosition = targetPosition;
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
                AbilityExecuting,
                Finished,
            }



            private IEnumerator<CrtState> _crt;
            private bool _stopped = false;

            public UpdatingExecute(Actor actor, Vector3 targetPosition, ProjectileAttackAbilityDef projectileAbility)
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

                IEnumerator<MainCrtState> mainCrt() 
                {
                    var navAgent = actor.GetComponent<NavAgent>();
                    var stopASAP = navAgent.StopASAPNavTarget;
                    navAgent.Target = stopASAP;

                    while (!stopASAP.HasArrived(actor.Pos)) {
                        yield return MainCrtState.Moving;
                    }

                    var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
                    while (!abilitiesComponent.IsEnabled(projectileAbility)) {
                        yield return MainCrtState.WaitingForAbilityEnabled;
                    }

                    abilitiesComponent.ActivateAbility(projectileAbility, targetPosition);

                    while (abilitiesComponent.Current != null 
                        && abilitiesComponent.Current.State.GeneralState == AbilityPlayer.GeneralExecutionState.Playing) {
                        yield return MainCrtState.AbilityExecuting;
                    }

                    yield return MainCrtState.Finished;
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
            return new UpdatingExecute(actor, _targetPosition, _abilityDef);
        }
    }
}
