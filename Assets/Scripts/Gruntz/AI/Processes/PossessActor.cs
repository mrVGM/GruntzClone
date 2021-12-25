using Base;
using Base.Actors;
using Base.Status;
using Base.UI;
using Gruntz.Statuses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.AI.Processes
{
    public class PossessActor : CoroutineProcess
    {
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var aiActor = behaviourTags.AIActor;
            var possessedActor = behaviourTags.PossessedActor;
            
            var game = Game.Instance;
            var actor = context.GetItem(aiActor) as Actor;

            var behaviourRoot = actor.ActorComponent.GetComponent<AIBehaviourRoot>();
            var actorManager = ActorManager.GetActorManagerFromContext();
            var actorIDStatusDef = game.DefRepositoryDef.AllDefs.OfType<ActorIDStatusDef>().FirstOrDefault();
            var possessed = actorManager.Actors.FirstOrDefault(x => {
                var statusComponent = x.GetComponent<StatusComponent>();
                var idStatus = statusComponent.GetStatus(actorIDStatusDef);
                var data = idStatus.StatusData as ActorIDStatusData;
                return data.ID == behaviourRoot.InitiallyPosessedActorID;
            });

            context.PutItem(possessedActor, possessed);
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
