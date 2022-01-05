using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.Equipment;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class AttackUnit : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag; 
        public MessagesBoxTagDef MessagesBoxTag;

        protected override IEnumerator<object> Crt()
        {
            var selected = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selected == null || !selected.Any())
            {
                yield break;
            }

            Actor getTargetActor()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var actorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.UnitSelection));
                if (actorHit.Equals(default(RaycastHit))) {
                    return null;
                }
                var actor = actorHit.collider.GetComponent<ActorProxy>().Actor;
                var team = actor.GetComponent<Team.TeamComponent>();
                if (team == null) {
                    return null;
                }
                if (team.UnitTeam == Team.TeamComponent.Team.Enemy) {
                    return actor;
                }
                return null;
            }

            var game = Game.Instance;

            while (Input.GetAxis("Ability") <= 0) {
                yield return null;
            }

            var targetActor = getTargetActor();
            if (targetActor == null) {
                while (Input.GetAxis("Ability") > 0) {
                    yield return null;
                }
                yield break;
            }

            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            foreach (var actor in selected) {
                var instruction = UnitController.Utils.GetAttackInstruction(actor, targetActor);
                if (instruction != null) {
                    messagesSystem.SendMessage(MessagesBoxTag,
                        MainUpdaterUpdateTime.Update,
                        this,
                        new UnitControllerInstruction {
                            Unit = actor,
                            Executable = instruction
                        });
                }
            }

            while (Input.GetAxis("Ability") > 0) {
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
