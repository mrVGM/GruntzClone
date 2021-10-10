using Base;
using Base.Actors;
using Base.MessagesSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Abilities
{
    public class AbilityDef : Def
    {
        public float Cooldown = 0;
        public AnimationClip Animation;
        public MessagesBoxTagDef AnimationEventMessages;
        public IEnumerator<object> Execute(Actor actor, object target)
        {
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            IEnumerable<AnimationEvent> eventsForMe()
            {
                var messages = messagesSystem.GetMessages(AnimationEventMessages);
                messages = messages.ToList();
                messages = messages.Where(x => x.Sender == actor);
                return messages.Select(x => x.Data as AnimationEvent);
            }
            while (!eventsForMe().Any(x => x.stringParameter == "ActionEnd")) {
                yield return null;
            }
        }
    }
}
