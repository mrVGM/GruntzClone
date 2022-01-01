using Base.Actors;
using Base.Gameplay;

namespace Gruntz.Gameplay.Actions
{
    public class PushActorAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public Actor ProjectileActor;
    }
}
