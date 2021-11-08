using Base.Gameplay;
using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class SpawnActorGenerator : GameplayActionGenerator
    {
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var spawnActorEvents = gameplayEvents.OfType<SpawnActorGameplayEvent>() ;

            foreach (var spawnActorEvent in spawnActorEvents) {
                yield return new SpawnActorAction {
                    Actor = null,
                    Pos = spawnActorEvent.Pos,
                    Template = spawnActorEvent.Template
                };
            }
        }
    }
}
