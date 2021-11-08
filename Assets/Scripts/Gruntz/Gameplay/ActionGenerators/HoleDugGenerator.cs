using Base.Gameplay;
using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class HoleDugGenerator : GameplayActionGenerator
    {
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var destroyActorEvents = gameplayEvents.OfType<HoleDugGameplayEvent>();

            foreach (var destroyedActorGameplayEvent in destroyActorEvents) {
                yield return new SwitchStateAction { 
                    Actor = destroyedActorGameplayEvent.TargetActor
                };
            }
        }
    }
}
