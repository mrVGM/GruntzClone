using Base.Actors;
using Gruntz.Actors;

namespace Gruntz.Gameplay
{
    public class ActorsClashedGameplayEvent : GameplayEvent
    {
        public Actor Actor1;
        public Actor Actor2;
    }
}
