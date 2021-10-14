using Base;
using Base.Actors;
using Base.Navigation;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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

        public static Actor DeployActorFromTemplate(ActorTemplateDef template, Vector3 pos)
        {
            var actorComponents = template.ActorComponents
                    .Select(x => new ActorData.Components { _component = x.ToDefRef<ActorComponentDef>() });

            var actorData = new ActorData {
                ActorDef = template.ActorDef.ToDefRef<ActorDef>(),
                ActorComponents = actorComponents.ToArray()
            };

            var navAgent = actorData.ActorComponents.FirstOrDefault(x => x.Component is NavAgentComponentDef);
            if (navAgent != null) {
                var navComponentDef = navAgent.Component as NavAgentComponentDef;
                NavAgentData navData = null;
                var formatter = new BinaryFormatter();
                using (var memStream = new MemoryStream())
                {
                    formatter.Serialize(memStream, navComponentDef.NavAgentData);
                    memStream.Position = 0;
                    navData = formatter.Deserialize(memStream) as NavAgentData;
                }
                navData.InitialPosition = pos;
                navData.Target = new NavAgent.SimpleNavTarget { Target = pos };
                navData.TravelSegmentStart = pos;
                navData.TravelSegmentEnd = pos;
                navAgent.Data = navData;
            }
            else {
                var repo = Game.Instance.DefRepositoryDef;
                var simplePosition = repo.AllDefs.OfType<SimplePositionComponentDef>().FirstOrDefault();
                actorData.ActorComponents =  actorData.ActorComponents.Append(new ActorData.Components {
                    _component = simplePosition.ToDefRef<ActorComponentDef>(),
                    Data = new SimplePositionComponentData { Position = pos }
                }).ToArray();
            }

            var actor = DeployActor(actorData);
            template.ProcessActor(actor);

            return actor;
        }
    }
}
