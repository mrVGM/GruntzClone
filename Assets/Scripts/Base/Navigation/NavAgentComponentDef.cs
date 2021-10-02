using Base.Actors;
using Base.MessagesSystem;

namespace Base.Navigation
{
    public class NavAgentComponentDef : ActorComponentDef
    {
        public MessagesBoxTagDef NavigationMessages;
        public NavAgentData NavAgentData;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            var navAgentBehaviour = actor.ActorComponent.GetComponent<NavAgentBehaviour>();
            var navAgent = new NavAgent(this, actor, NavAgentData, navAgentBehaviour);
            return navAgent;
        }
    }
}
