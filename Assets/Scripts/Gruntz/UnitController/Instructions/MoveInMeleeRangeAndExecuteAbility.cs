using Base.Actors;
using Base.Navigation;
using Gruntz.Abilities;
using Gruntz.UnitController;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UnitController.Instructions
{
    public class MoveInMeleeRangeAndExecuteAbility : IUnitExecutable
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

        public MoveInMeleeRangeAndExecuteAbility(Actor targetActor, AbilityDef ability)
        {
            _targetActor = targetActor;
            _ability = ability;
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

            public UpdatingExecute(Actor actor, Actor targetActor, AbilityDef ability)
            {
                var navigation = Navigation.GetNavigationFromContext();
                var map = navigation.Map;
                var navAgent = actor.GetComponent<NavAgent>();
                var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();

                IEnumerator<CrtState> crt()
                {
                    while (true) {
                        if (!targetActor.IsInPlay) {
                            break;
                        }
                        var navTarget = new NeighboursTarget { Pos = map.SnapPosition(targetActor.Pos) };
                        navAgent.NavTarget = navTarget;
                        if (navTarget.HasArrived(actor.Pos)) {
                            navAgent.TurnTo(targetActor.Pos);
                            if (abilitiesComponent.IsEnabled(ability)) {
                                if (abilitiesComponent.CanExecuteOn(ability, targetActor)) {
                                    abilitiesComponent.ActivateAbility(ability, targetActor);
                                    while (abilitiesComponent.Current != null
                                        && abilitiesComponent.Current.State.GeneralState == AbilityPlayer.GeneralExecutionState.Playing) {
                                        yield return CrtState.Active;
                                    }
                                }
                                break;
                            }
                        }
                        yield return CrtState.Active;
                    }
                    yield return CrtState.Finished;
                }

                IEnumerator<CrtState> stoppableCrt()
                {
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
                            if (abilitiesComponent.Current != null) {
                                abilitiesComponent.Current.Interrupt();
                            }
                            break;
                        }
                        yield return coroutine.Current;
                    }
                    yield return coroutine.Current;
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
            return new UpdatingExecute(actor, _targetActor, _ability);
        }
    }
}
