using Base.Actors;

namespace Gruntz.Gameplay.Actions
{
    public class DamageActorAction : IGameplayAction
    {
        public Actor Actor { get; set; }
        public float DamageValue;
        public Actor DamageDealer;
    }
}
