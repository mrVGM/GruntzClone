using Gruntz.Navigation;
using UnityEngine;

namespace Gruntz.Actors
{
    public static class ActorDeployment
    {
        public static Actor DeployActor(ActorData actorData, Vector3 deployPosition)
        {
            var actorComponent = Object.Instantiate(actorData.ActorPrefab);
            var actor = new Actor(actorComponent, actorData);
            actor.Init();

            var nav = actor.GetComponent<NavAgent>();
            var navData = nav.Data as NavAgentData;
            navData.InitialPosition = deployPosition;
            navData.Target = deployPosition;
            nav.Data = navData;
            return actor;
        }
    }
}
