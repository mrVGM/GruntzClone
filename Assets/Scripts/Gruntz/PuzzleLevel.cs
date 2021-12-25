using Base;
using Base.Actors;
using Base.Navigation;
using Base.Status;
using Gruntz.Actors;
using Gruntz.AI;
using Gruntz.Statuses;
using Gruntz.Team;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using static Base.MainUpdaterLock;

namespace Gruntz
{
    public class PuzzleLevel : MonoBehaviour
    {
        public SceneIDs SceneIDs;
        public Transform ActorDeployPoints;

        List<ILock> _locks;

        private void SetupActorManager()
        {
            var game = Game.Instance;
            var sceneIDsHolder = SceneIDsHolder.GetSceneIDsHolderFromContext();
            sceneIDsHolder.SceneIDs = SceneIDs;

            var actorDeployPoints = ActorDeployPoints.GetComponentsInChildren<ActorDeployPoint>();
            foreach (var dp in actorDeployPoints) {
                for (int i = 0; i < dp.transform.childCount; ++i) {
                    var child = dp.transform.GetChild(i);
                    child.gameObject.SetActive(false);
                }
            }

            var actorManager = ActorManager.GetActorManagerFromContext();
            var savedGameHolder = SavedGameHolder.GetSavedGameHolderFromContext();
            if (savedGameHolder.SavedGame != null)
            {
                actorManager.DeployActor = ActorDeployment.DeployActor;
                savedGameHolder.RestoreContext();
                actorManager.DeployActor = null;
                return;
            }

            var deployDatas = new Dictionary<ActorData, ActorDeployPoint>();
            foreach (var deployPoint in actorDeployPoints)
            {
                var actorComponents = deployPoint.ActorTempleteDef.Components
                    .Select(x => new ActorData.Components { _component = x.ToDefRef<ActorComponentDef>() });
                if (deployPoint.ActorComponent != null)
                {
                    deployPoint.ActorComponent.gameObject.SetActive(true);
                    var defRepo = game.DefRepositoryDef;
                    var sceneIDComponentDef = defRepo.AllDefs.OfType<SceneIDComponentDef>().FirstOrDefault();
                    var data = new SceneIDComponentData { ID = SceneIDs.SceneObjectIDs.FirstOrDefault(x => x.GameObject == deployPoint.ActorComponent.gameObject).ID };
                    actorComponents = actorComponents.Prepend(new ActorData.Components { _component = sceneIDComponentDef.ToDefRef<ActorComponentDef>(), Data = data });
                }
                var actorData = new ActorData
                {
                    ActorDef = (deployPoint.ActorTempleteDef.ActorPrefabDef != null) ? deployPoint.ActorTempleteDef.ActorPrefabDef.ToDefRef<ActorDef>() : default(DefRef<ActorDef>),
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
                    navData.Target = new NavAgent.SimpleNavTarget { Target = deployPoint.transform.position };
                    navData.Speed = navComponentDef.NavAgentData.Speed;
                    navComponent.Data = navData;
                }
                deployDatas[actorData] = deployPoint;
            }
            var actorManagerData = new ActorManagerData { ActorDatas = deployDatas.Keys.ToList() };

            var actorIDStatusDef = game.DefRepositoryDef.AllDefs.OfType<ActorIDStatusDef>().FirstOrDefault();

            actorManager.DeployActor = x => {
                var actor = ActorDeployment.DeployActor(x);
                var deployPoint = deployDatas[actor.Data as ActorData];
                deployPoint.ActorTempleteDef.ProcessActor(actor);
                var team = deployPoint.GetComponent<DeployPointTeam>();
                if (team != null) {
                    var teamComponent = actor.GetComponent<TeamComponent>();
                    teamComponent.UnitTeam = team.Team;
                }
                var statusComponent = actor.GetComponent<StatusComponent>();
                var statusData = actorIDStatusDef.Data as ActorIDStatusData;
                var status = statusData.CreateStatus();
                statusComponent.AddStatus(status);
                var deployPointID = deployPoint.GetComponent<ActorDeployPointID>();
                if (deployPointID != null) {
                    statusData.ID = deployPointID.ID;
                }
                return actor;
            };
            actorManager.Data = actorManagerData;
            actorManager.DeployActor = null;
        }
        public void LevelLoaded()
        {
            var game = Game.Instance;
            _locks = new List<ILock>();
            var orderTagDef = game.DefRepositoryDef.AllDefs.OfType<LevelLoadedOrderTagDef>().First();
            foreach (var tag in game.MainUpdater.ExecutionOrder)
            {
                if (tag == orderTagDef)
                {
                    continue;
                }
                var l = game.MainUpdater.MainUpdaterLock.TryLock(tag);
                _locks.Add(l);
            }

            SetupActorManager();

            foreach (var l in _locks)
            {
                game.MainUpdater.MainUpdaterLock.Unlock(l);
            }
        }
    }
}
