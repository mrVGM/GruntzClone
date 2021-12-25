using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.AI.Processes
{
    public class ActorDead : CoroutineProcess
    {
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            Debug.Log($"Actor {possessedActor.ActorComponent} died!");
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
