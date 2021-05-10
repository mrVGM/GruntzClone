using Base;
using System.Collections.Generic;

namespace Gruntz.Actors
{
    public class ActorManager : IContextObject
    {
        private List<Actor> actors { get; } = new List<Actor>();
        public IEnumerable<Actor> Actors => actors;
        public void AddActor(Actor actor)
        {
            actors.Add(actor);
            actor.Init();
        }
        public void DisposeObject()
        {
        }
    }
}
