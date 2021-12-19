using Base.Actors;
using Base.MessagesSystem;
using Base.Status;
using Gruntz.Statuses;

namespace Gruntz.AI
{
    public class AIComponentDef : ActorComponentDef
    {
        public SceneIDStatusDef AIControllerSceneIDStatusDef;

        public StatusDef RegularActorStatusDef;
        public MessagesBoxTagDef MessagesBox;
        public float UpdateInterval = 0.5f;
        public float Range;

        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AIComponent(this, actor);
        }
    }
}
