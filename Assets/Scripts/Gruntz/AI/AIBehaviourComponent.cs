using Base.Actors;

namespace Gruntz.AI
{
    public class AIBehaviourComponent : IActorComponent
    {
        public Actor Actor { get; }
        public AIBehaviourComponentDef AIProcessesComponentDef;
        
        public AIBehaviourComponent(Actor actor, AIBehaviourComponentDef aiScriptsComponentDef)
        {
            Actor = actor;
            AIProcessesComponentDef = aiScriptsComponentDef;
        }

        public void DeInit()
        {
        }

        public void Init()
        {
        }
    }
}
