using Base.Actors;
using Base.Navigation;
using Gruntz.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UnitController.Instructions
{
    public class AttackUnit : IUnitExecutable
    {
        [Serializable]
        public struct NeighboursTarget : INavigationTarget
        {
            public Map Map => Navigation.GetNavigationFromContext().Map;
            public SerializedVector3 Pos;
            public Vector3 AdjustPosition(Vector3 pos)
            {
                var neighbours = Map.GetNeighbours(Pos);
                var closest = neighbours.OrderBy(x => (x - pos).sqrMagnitude).FirstOrDefault();
                return closest;
            }

            public bool HasArrived(Vector3 pos)
            {
                var neighbours = Map.GetNeighbours(Pos);
                var closest = neighbours.OrderBy(x => (x - pos).sqrMagnitude).FirstOrDefault();
                if ((pos - closest).sqrMagnitude < Navigation.Eps) {
                    return true;
                }
                return false;
            }

            public float Proximity(Vector3 pos)
            {
                var neighbours = Map.GetNeighbours(Pos);
                return neighbours.Min(x => (x - pos).sqrMagnitude);
            }
        }

        private Actor _targetActor;
        private AbilityDef _ability;

        public AttackUnit(Actor targetActor)
        {
            _targetActor = targetActor;
        }

        private class UpdatingExecute : IUpdatingExecutable
        {
            private enum CrtState
            {
                Active,
                Finished,
                Interrupted
            }

            private IEnumerator<CrtState> _crt;
            private bool _stopped = false;

            public UpdatingExecute(Actor actor, Actor targetActor)
            {
                var navigation = Navigation.GetNavigationFromContext();
                var map = navigation.Map;
                var navAgent = actor.GetComponent<NavAgent>();
                var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();

                var conflictManager = ConflictManager.ConflictManager.GetConflictManagerFromContext();

                IEnumerator<CrtState> crt()
                {
                    while (true) {
                        if (!targetActor.IsInPlay) {
                            break;
                        }
                        var navTarget = new NeighboursTarget { Pos = map.SnapPosition(targetActor.Pos) };
                        navAgent.NavTarget = navTarget;
                        bool waitFrame = true;
                        if (navTarget.HasArrived(actor.Pos)) {
                            navAgent.TurnTo(targetActor.Pos);
                            var ability = abilitiesComponent.GetAttackAbility();
                            if (ability == null) {
                                break;
                            }
                            if (abilitiesComponent.IsEnabled(ability)) {
                                abilitiesComponent.ActivateAbility(ability, targetActor);

                                while (abilitiesComponent.Current != null
                                    && abilitiesComponent.Current.State.GeneralState == AbilityPlayer.GeneralExecutionState.Playing) {
                                    yield return CrtState.Active;
                                    waitFrame = false;
                                }
                            }
                        }
                        if (waitFrame) {
                            yield return CrtState.Active;
                        }
                    }
                    yield return CrtState.Finished;
                }

                IEnumerator<CrtState> stoppableCrt()
                {
                    var unitController = actor.GetComponent<UnitController>();
                    unitController.UnitControllerState.FightingWith = targetActor;

                    var coroutine = crt();
                    while (true) {
                        if (_stopped) {
                            yield return CrtState.Interrupted;
                            break;
                        }
                        coroutine.MoveNext();
                        yield return coroutine.Current;
                        if (coroutine.Current == CrtState.Finished) {
                            break;
                        }
                    }
                }

                IEnumerator<CrtState> realCrt()
                {
                    var coroutine = stoppableCrt();
                    while (true) {
                        coroutine.MoveNext();
                        if (coroutine.Current != CrtState.Active) {
                            var unitController = actor.GetComponent<UnitController>();
                            unitController.UnitControllerState.FightingWith = null;

                            if (coroutine.Current == CrtState.Interrupted && abilitiesComponent.Current != null) {
                                abilitiesComponent.Current.Interrupt();
                            }
                            yield return coroutine.Current;
                            break;
                        }

                        yield return coroutine.Current;
                    }
                }

                _crt = realCrt();
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
            return new UpdatingExecute(actor, _targetActor);
        }
    }
}
