using Base.Actors;
using Base.MessagesSystem;
using UnityEngine;

namespace Base.Animations
{
    public class AnimationsComponentDef : ActorComponentDef
    {
        public MessagesBoxTagDef NavigationMessages;
        public MessagesBoxTagDef AbilityMessages;
        public AnimationClip DefaultAbilityAnimation;
        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new AnimationsComponent(actor, this);
        }
    }
}
