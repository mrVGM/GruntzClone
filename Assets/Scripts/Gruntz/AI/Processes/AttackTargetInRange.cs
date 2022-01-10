using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using Gruntz.Team;
using Gruntz.UnitController.Instructions;

namespace Gruntz.AI.Processes
{
    public class AttackTargetInRange : CoroutineProcess
    {
        public ProcessContextTagDef AttackRangeTag;
        public ProcessContextTagDef InstructionsAccumulatedDef;
        
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

                float range = (float)context.GetItem(AttackRangeTag);
                var targetActor = actors.FirstOrDefault(x => (possessedActor.Pos - x.Pos).magnitude < range);
                if (targetActor != null) {
                    List<IUnitExecutable> instructionList = context.GetItem(InstructionsAccumulatedDef) as List<IUnitExecutable>;
                    if (instructionList == null) {
                        instructionList = new List<IUnitExecutable>();
                        context.PutItem(InstructionsAccumulatedDef, instructionList);
                    }
                    instructionList.Add(new AttackUnit(targetActor));
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
