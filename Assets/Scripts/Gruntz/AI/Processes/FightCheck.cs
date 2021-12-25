using Base.Actors;
using Base.UI;
using System.Collections.Generic;

namespace Gruntz.AI.Processes
{
    public class FightCheck : CoroutineProcess
    {
        public bool Fighting;
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            var unitController = possessedActor.GetComponent<UnitController.UnitController>();

            if (!Fighting) {
                while (unitController.UnitControllerState.FightingWith == null) {
                    yield return null;
                }
            }
            else {
                while (unitController.UnitControllerState.FightingWith != null) {
                    yield return null;
                }
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
