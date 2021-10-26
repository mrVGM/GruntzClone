using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.Navigation;
using Gruntz.Team;
using UnityEngine;

namespace Gruntz.UI.ActorControl
{
    public static class ActorControlUtils
    {
        public static bool CanStepToPosition(IEnumerable<NavAgent> agent, Vector3 pos)
        {
            var layers = agent.SelectMany(x => {
                var navAgentData = x.Data as NavAgentData;
                return navAgentData.ObstacleNames;
            });
            var layerMask = LayerMask.GetMask(layers.Distinct().ToArray());
            var hits = Physics.OverlapSphere(pos, 0.001f, layerMask);
            if (hits.Any())
            {
                return false;
            }
            return true;
        }

        public static bool CanSelectActor(Actor actor, IEnumerable<Actor> alreadySelected)
        {
            var teamComponent = actor.GetComponent<TeamComponent>();
            if (teamComponent == null) {
                return false;
            }

            if (teamComponent.UnitTeam != TeamComponent.Team.Player) {
                return false;
            }

            if (alreadySelected != null && alreadySelected.Contains(actor)) {
                return false;
            }

            return true;
        }
    }
}
