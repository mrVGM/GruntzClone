using Base.Actors;
using Base.Gameplay;

namespace Gruntz.Gameplay
{
    public class ActorsClashedGameplayEvent : GameplayEvent
    {
        public Actor Actor1;
        public Actor Actor2;
    }
}
