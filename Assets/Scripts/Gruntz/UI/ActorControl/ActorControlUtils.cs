using System.Collections.Generic;
using System.Linq;
using Base.Navigation;
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
    }
}
