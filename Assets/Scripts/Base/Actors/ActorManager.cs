using System;
using System.Collections.Generic;
using System.Linq;

namespace Base.Actors
{
    public class ActorManager : IContextObject, ISerializedObject
    {
        private List<Actor> actors { get; } = new List<Actor>();
        public ActorManagerDef ActorManagerDef;
        public IEnumerable<Actor> Actors => actors;

        public Func<ActorData, Actor> DeployActor;

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
                    DeployActor(actorData);
                }
            }
        }

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
        }
        public void RemoveActor(Actor actor)
        {
            actors.Remove(actor);
        }
        public void DisposeObject()
        {
            foreach (var actor in actors.ToList()) {
                actor.Deinit();
            }
        }

        public static ActorManager GetActorManagerFromContext()
        {
            var game = Game.Instance;
            var actorManagerDef = game.DefRepositoryDef.AllDefs.OfType<ActorManagerDef>().First();
            var actorManager = game.Context.GetRuntimeObject(actorManagerDef) as ActorManager;
            actorManager.ActorManagerDef = actorManagerDef;
            return actorManager;
        }
    }
}
