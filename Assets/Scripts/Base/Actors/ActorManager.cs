using Base;
using Gruntz.Actors;
using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;

namespace Base.Actors
{
    public class ActorManager : IContextObject, ISerializedObject
    {
        private List<Actor> actors { get; } = new List<Actor>();
        public IEnumerable<Actor> Actors => actors;

        public ISerializedObjectData Data
        {
            get
            {
                return new ActorManagerData { ActorDatas = Actors.Select(x => x.Data as ActorData).ToList() };
            }
            set
            {
                var actorManagerData = value as ActorManagerData;
                foreach (var actorData in actorManagerData.ActorDatas)
                {
                    var statusComponent = actorData.ActorComponents.FirstOrDefault(x => x.Component is StatusComponentDef);
                    if (statusComponent != null) {
                        var statusComponentData = statusComponent.Data as StatusComponentData;
                        if (statusComponentData != null)  {
                            var healthStatusData = statusComponentData.StatusDatas.OfType<HealthStatusData>().FirstOrDefault();
                            if (healthStatusData != null && healthStatusData.Health <= 0) {
                                continue;
                            }
                        }
                    }

                    ActorDeployment.DeployActor(actorData);
                }
            }
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }
        public void DisposeObject()
        {
            foreach (var actor in actors)
            {
                actor.Deinit();
            }
        }

        public static ActorManager GetActorManagerFromContext()
        {
            var game = Game.Instance;
            var actorManagerDef = game.DefRepositoryDef.AllDefs.OfType<ActorManagerDef>().First();
            var actorManager = game.Context.GetRuntimeObject(actorManagerDef) as ActorManager;
            return actorManager;
        }
    }
}
