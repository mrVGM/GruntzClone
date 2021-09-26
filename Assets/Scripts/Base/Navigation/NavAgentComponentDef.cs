using Base.Actors;

namespace Base.Navigation
{
    public class NavAgentComponentDef : ActorComponentDef
    {
        public NavAgentData NavAgentData;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            var navAgentBehaviour = actor.ActorComponent.GetComponent<NavAgentBehaviour>();
            var navAgent = new NavAgent(actor, NavAgentData, navAgentBehaviour);
            return navAgent;
        }
    }
}
