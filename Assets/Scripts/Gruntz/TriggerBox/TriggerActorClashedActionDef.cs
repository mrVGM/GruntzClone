using Base.Actors;
using Base.Gameplay;
using Gruntz.Actors;
using Gruntz.Gameplay;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class TriggerActorClashedActionDef : TriggerBoxActionDef
    {
        public override void TriggerEnter(Collider ownCollider, Collider otherCollider)
        {
            if (otherCollider.GetComponent<ActorBody>() != null) {
                var ownActor = ownCollider.GetComponent<ActorProxy>().Actor;
                var otherActor = otherCollider.GetComponent<ActorProxy>().Actor;
                if (ownActor == otherActor) {
                    return;
                }
                var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                gameplayManager.HandleGameplayEvent(new ActorsClashedGameplayEvent { Actor1 = ownActor, Actor2 = otherActor });
            }
        }

        public override void TriggerExit(Collider ownCollider, Collider otherCollider)
        {
        }
    }
}
