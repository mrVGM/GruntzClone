using Base.Actors;
using Base.Gameplay;

namespace Gruntz.Gameplay
{
    public class ProjectileHitActorGameplayEvent : GameplayEvent
    {
        public Actor ProjectileActor;
        public Actor ActorHit;
    }
}
