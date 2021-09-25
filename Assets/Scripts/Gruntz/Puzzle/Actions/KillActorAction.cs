using Gruntz.Actors;

namespace Gruntz.Puzzle.Actions
{
    public class KillActorAction : IGameplayAction
    {
        public Actor Actor { get; set; }

        public void Execute()
        {
            Actor.Die();
        }
    }
}
