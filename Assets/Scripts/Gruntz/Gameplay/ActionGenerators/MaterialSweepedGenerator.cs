using Base.Gameplay;
using Gruntz.Gameplay.Actions;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Gameplay.ActionGenarators
{
    public class MaterialSweepedGenerator : GameplayActionGenerator
    {
        public override IEnumerable<IGameplayAction> GenerateActions(IEnumerable<GameplayEvent> gameplayEvents)
        {
            var materialSweepedEvents = gameplayEvents.OfType<MaterialSweepedGameplayEvent>();

            foreach (var materialSweepedEvent in materialSweepedEvents) {
                yield return new KillActorAction {
                    Actor = materialSweepedEvent.TargetActor,
                    Reason = KillActorAction.DeathReason.Destruction,
                };
                yield return new CollectMaterialAction {
                    Actor = materialSweepedEvent.SourceActor
                };
            }
        }
    }
}
