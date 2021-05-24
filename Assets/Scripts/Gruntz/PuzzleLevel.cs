using Base;
using Gruntz.Actors;
using System.Linq;
using UnityEngine;

namespace Gruntz
{
    public class PuzzleLevel : MonoBehaviour
    {
        public Transform ActorContainer;
        public void LevelLoaded()
        {
            var actorDeployPoints = ActorContainer.GetComponentsInChildren<ActorDeployPoint>();
            foreach (var deployPoint in actorDeployPoints)
            {
                ActorDeployment.DeployActor(deployPoint.ActorData, deployPoint.transform.position);
            }
        }
    }
}
