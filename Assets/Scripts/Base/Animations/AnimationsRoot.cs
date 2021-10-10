using Base.Actors;
using Base.MessagesSystem;
using UnityEngine;

namespace Base.Animations
{
    public class AnimationsRoot : MonoBehaviour
    {
        public MessagesBoxTagDef MessagesBox;
        public void OnAnimationEvent(AnimationEvent animationEvent)
        {
            var messagesSystem = MessagesSystem.MessagesSystem.GetMessagesSystemFromContext();
            Actor actor = null;
            var actorProxy = GetComponent<ActorProxy>();
            if (actorProxy != null) {
                actor = actorProxy.Actor;
            }

            messagesSystem.SendMessage(MessagesBox, MainUpdaterUpdateTime.FixedCrt, actor, animationEvent);
        }
    }
}
