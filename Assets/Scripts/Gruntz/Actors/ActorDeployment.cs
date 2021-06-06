using Gruntz.Navigation;
using UnityEngine;

namespace Gruntz.Actors
{
    public static class ActorDeployment
    {
        public static Actor DeployActor(ActorData actorData)
        {
            var actorComponent = Object.Instantiate(actorData.ActorPrefab);
            var actor = new Actor(actorComponent, actorData);
            actor.Init();

            return actor;
        }
    }
}
