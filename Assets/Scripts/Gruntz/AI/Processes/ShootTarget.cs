using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using Gruntz.Team;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using Gruntz.Abilities;

namespace Gruntz.AI.Processes
{
    public class ShootTarget : CoroutineProcess
    {
        public MessagesBoxTagDef MessagesBoxTag;
        public float Range = 7;
        
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;

            var abilitiesComponent = possessedActor.GetComponent<AbilitiesComponent>();
            var projectileAttackAbility = abilitiesComponent.GetAttackAbility() as ProjectileAttackAbilityDef;

            var actorManager = ActorManager.GetActorManagerFromContext();
            while (true) {
                while (!abilitiesComponent.IsEnabled(projectileAttackAbility)) {
                    yield return null;
                }

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

                if (targetActor == null) {
                    yield return null;
                    continue;
                }
                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                messagesSystem.SendMessage(
                    MessagesBoxTag,
                    MainUpdaterUpdateTime.Update,
                    this,
                    new UnitControllerInstruction {
                        Unit = possessedActor,
                        Executable = new StopAndShootProjectile(targetActor.Pos, projectileAttackAbility),
                    });

                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
