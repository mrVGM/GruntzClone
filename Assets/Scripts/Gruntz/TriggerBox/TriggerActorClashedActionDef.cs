using Gruntz.Actors;
using Gruntz.Puzzle;
using UnityEngine;

namespace Gruntz
{
    public class TriggerActorClashedActionDef : TriggerBoxActionDef
    {
        public override void TriggerEnter(Collider ownCollider, Collider otherCollider)
        {
            if (otherCollider.GetComponent<ActorBody>() != null) {
                var ownActor = ownCollider.GetComponent<ActorProxy>().Actor;
                var otherActor = otherCollider.GetComponent<ActorProxy>().Actor;
                var gameplayManager = GameplayManager.GetActorManagerFromContext();
                gameplayManager.HandleGameplayEvent(new ActorsClashedGameplayEvent { Actor1 = ownActor, Actor2 = otherActor });
            }
        }

        public override void TriggerExit(Collider ownCollider, Collider otherCollider)
        {
        }
    }
}
