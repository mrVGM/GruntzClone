using Gruntz.Actors;
using Gruntz.Navigation;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Gruntz
{
    public class PuzzleLevel : MonoBehaviour
    {
        public Transform ActorDeployPoints;
        public void LevelLoaded()
        {
            var actorDeployPoints = ActorDeployPoints.GetComponentsInChildren<ActorDeployPoint>();
            var deployDatas = new List<ActorData>();

            foreach (var deployPoint in actorDeployPoints)
            {
                var actorData = deployPoint.ActorData;
                var navComponent = actorData.ActorComponents.First(x => x.Component is NavAgentComponentDef);
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
                deployDatas.Add(actorData);
            }
            var actorManagerData = new ActorManagerData { ActorDatas = deployDatas };

            var actorManager = ActorManager.GetActorManagerFromContext();
            actorManager.Data = actorManagerData;
        }
    }
}
