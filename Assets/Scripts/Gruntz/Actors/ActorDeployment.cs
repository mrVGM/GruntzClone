using Base;
using Base.Actors;
using System.Linq;
using UnityEngine;

namespace Gruntz.Actors
{
    public static class ActorDeployment
    {
        public static Actor DeployActor(ActorData actorData)
        {
            ActorComponent actorComponent = null;
            var sceneID = actorData.ActorComponents.FirstOrDefault(x => ((Def)x._component) is SceneIDComponentDef);
            if (sceneID != null)
            {
                var sceneIDsHolder = SceneIDsHolder.GetSceneIDsHolderFromContext();
                var sceneIDs = sceneIDsHolder.SceneIDs;
                var actorComponentGO = sceneIDs.SceneObjectIDs.FirstOrDefault(x => x.ID == (sceneID.Data as SceneIDComponentData).ID);
                actorComponent = actorComponentGO.GameObject.GetComponent<ActorComponent>();
            }
            if (actorComponent == null)
            {
                actorComponent = Object.Instantiate(actorData.ActorPrefab);
            }
            var actor = new Actor(actorComponent, actorData);
            actor.Init();

            return actor;
        }
    }
}
