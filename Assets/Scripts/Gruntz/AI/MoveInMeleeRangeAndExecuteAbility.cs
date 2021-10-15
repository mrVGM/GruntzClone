using Base.Actors;
using Base.Navigation;
using Gruntz.Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.AI
{
    public class MoveInMeleeRangeAndExecuteAbility : IAIAction
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

        private enum CrtState
        {
            Active,
            Finished
        }

        IEnumerator<CrtState> _crt;
        public MoveInMeleeRangeAndExecuteAbility(Actor actor, Actor targetActor, AbilityDef ability)
        {
            var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
            var navAgent = actor.GetComponent<NavAgent>();
            var navigation = Navigation.GetNavigationFromContext();
            var map = navigation.Map;
            navAgent.NavTarget = new NeighboursTarget { Pos = targetActor.Pos };

            IEnumerator<CrtState> crt()
            {
                yield return CrtState.Active;
                while(true) {
                    var navTarget = new NeighboursTarget { Pos = map.SnapPosition(targetActor.Pos) };
                    navAgent.NavTarget = navTarget;
                    if (navTarget.HasArrived(actor.Pos)) {
                        navAgent.TurnTo(targetActor.Pos);
                        if (abilitiesComponent.IsEnabled(ability)) {
                            abilitiesComponent.ActivateAbility(ability, targetActor);
                            break;
                        }
                    }
                    yield return CrtState.Active;
                }
                yield return CrtState.Finished;
            }
            _crt = crt();
            _crt.MoveNext();
        }
        public bool CanProceed()
        {
            return _crt.Current == CrtState.Active;
        }

        public void Update()
        {
            _crt.MoveNext();
        }

        public void Stop()
        {
        }
    }
}
