using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using Base.Navigation;
using Gruntz.Team;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using UnityEngine;

namespace Gruntz.AI.Processes
{
    public class AttackTargetInRange : CoroutineProcess
    {
        public float Range;
        public MessagesBoxTagDef MessagesBoxTag;
        
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            
            var actorManager = ActorManager.GetActorManagerFromContext();
            while (true) {
                var actors = actorManager.Actors.Where(x =>
                {
                    var team = x.GetComponent<TeamComponent>();
                    if (team == null)
                    {
                        return false;
                    }

                    return team.UnitTeam == TeamComponent.Team.Player;
                });
                var targetActor = actors.FirstOrDefault(x => (possessedActor.Pos - x.Pos).magnitude < Range);
                if (targetActor != null) {
                    var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                    messagesSystem.SendMessage(
                        MessagesBoxTag,
                        MainUpdaterUpdateTime.Update,
                        this,
                        new UnitControllerInstruction {
                            Unit = possessedActor,
                            Executable = new AttackUnit(targetActor),
                        });
                    yield break;
                }

                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
