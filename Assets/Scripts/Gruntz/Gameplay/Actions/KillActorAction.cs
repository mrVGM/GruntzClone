using Base.Actors;
using Base.Gameplay;

namespace Gruntz.Gameplay.Actions
{
    public class KillActorAction : IGameplayAction
    {
        public enum DeathReason
        {
            Clash,
            Damage,
            Destruction
        }
        public Actor Actor { get; set; }
        public DeathReason Reason = DeathReason.Destruction;
    }
}
