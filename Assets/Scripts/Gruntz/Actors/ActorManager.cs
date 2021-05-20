using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Actors
{
    public class ActorManager : IContextObject
    {
        private List<Actor> actors { get; } = new List<Actor>();
        public IEnumerable<Actor> Actors => actors;
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
