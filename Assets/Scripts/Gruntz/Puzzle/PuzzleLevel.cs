using Base;
using Gruntz.Actors;
using Gruntz.Navigation;
using Gruntz.SceneID;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Gruntz.Puzzle
{
    public class PuzzleLevel : MonoBehaviour
    {
        public SceneIDs SceneIDs;
        public Transform ActorDeployPoints;
        public void LevelLoaded()
        {
            var game = Game.Instance;
            var sceneIDsHolder = SceneIDsHolder.GetSceneIDsHolderFromContext();
            sceneIDsHolder.SceneIDs = SceneIDs;

            var savedGameHolder = SavedGameHolder.GetSavedGameHolderFromGame();
            if (savedGameHolder.SavedGame != null)
            {
                foreach (var pair in savedGameHolder.SavedGame.SerializedContextObjects)
                {
                    var contextObject = game.Context.GetRuntimeObject(pair.Def);
                    var serializedObject = contextObject as ISerializedObject;
                    serializedObject.Data = pair.ContextObjectData;
                }
                return;
            }

            var actorDeployPoints = ActorDeployPoints.GetComponentsInChildren<ActorDeployPoint>();
            var deployDatas = new List<ActorData>();

            foreach (var deployPoint in actorDeployPoints)
            {
                var actorComponents = deployPoint.ActorDeployDef.ActorComponents
                    .Select(x => new ActorData.Components { _component = x.ToDefRef<ActorComponentDef>() });
                if (deployPoint.ActorComponent != null)
                {
                    var defRepo = game.DefRepositoryDef;
                    var sceneIDComponentDef = defRepo.AllDefs.OfType<SceneIDComponentDef>().FirstOrDefault();
                    var data = new SceneIDComponentData { ID = SceneIDs.SceneObjectIDs.FirstOrDefault(x => x.GameObject == deployPoint.ActorComponent.gameObject).ID };
                    actorComponents = actorComponents.Prepend(new ActorData.Components { _component = sceneIDComponentDef.ToDefRef<ActorComponentDef>(), Data = data });
                }
                var actorData = new ActorData {
                    ActorDef = (deployPoint.ActorDeployDef.ActorDef != null) ? deployPoint.ActorDeployDef.ActorDef.ToDefRef<ActorDef>() : default(DefRef<ActorDef>),
                    ActorComponents = actorComponents.ToArray()
                };
                var navComponent = actorData.ActorComponents.FirstOrDefault(x => x.Component is NavAgentComponentDef);
                if (navComponent != null)
                {
                    var navComponentDef = navComponent.Component as NavAgentComponentDef;
                    NavAgentData navData = null;
                    var formatter = new BinaryFormatter();
                    using (var memStream = new MemoryStream())
                    {
                        formatter.Serialize(memStream, navComponentDef.NavAgentData);
                        memStream.Position = 0;
                        navData = formatter.Deserialize(memStream) as NavAgentData;
                    }
                    navData.InitialPosition = deployPoint.transform.position;
                    navData.Target = deployPoint.transform.position;
                    navData.Speed = navComponentDef.NavAgentData.Speed;
                    navComponent.Data = navData;
                }
                deployDatas.Add(actorData);
            }
            var actorManagerData = new ActorManagerData { ActorDatas = deployDatas };

            var actorManager = ActorManager.GetActorManagerFromContext();
            actorManager.Data = actorManagerData;
        }
    }
}
