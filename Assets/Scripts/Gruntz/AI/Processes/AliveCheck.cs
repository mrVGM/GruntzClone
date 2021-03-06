using Base.Actors;
using Base.UI;
using System.Collections.Generic;

namespace Gruntz.AI.Processes
{
    public class AliveCheck : CoroutineProcess
    {
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            while (possessedActor != null &&  possessedActor.IsInPlay) {
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
