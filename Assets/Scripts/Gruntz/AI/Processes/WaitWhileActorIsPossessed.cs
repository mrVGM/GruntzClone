using Base;
using Base.Actors;
using Base.Status;
using Base.UI;
using System.Collections.Generic;

namespace Gruntz.AI.Processes
{
    public class WaitWhileActorIsPossessed : CoroutineProcess
    {
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActor = behaviourTags.PossessedActor;

            while (true) {
                var actor = context.GetItem(possessedActor) as Actor;
                if (actor != null) {
                    break;
                }
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
