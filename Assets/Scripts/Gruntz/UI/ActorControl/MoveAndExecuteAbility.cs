using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class MoveAndExecuteAbility : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag; 
        public MessagesBoxTagDef MessagesBoxTag;

        protected override IEnumerator<object> Crt()
        {
            var selected = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selected == null || selected.Count() != 1) {
                yield break;
            }

            Actor getTargetActor()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var actorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.ActorGeneral));
                if (actorHit.Equals(default(RaycastHit))) {
                    return null;
                }
                return actorHit.collider.GetComponent<ActorProxy>().Actor;
            }

            var game = Game.Instance;

            while (Input.GetAxis("Ability") > 0) {
                yield return null;
            }

            while (Input.GetAxis("Ability") <= 0) {
                yield return null;
            }

            var selectedActor = selected.First();
            var abilitiesComponent = selectedActor.GetComponent<AbilitiesComponent>();

            var ability = abilitiesComponent.GetMainAbility();
            var targetActor = getTargetActor();
            
            while (Input.GetAxis("Ability") > 0) {
                yield return null;
            }
            if (targetActor != getTargetActor()) {
                yield break;
            }

            if (targetActor == null || !abilitiesComponent.CanExecuteOn(ability, targetActor)) {
                yield break;
            }

            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            var instruction = new MoveInMeleeRangeAndExecuteAbility(targetActor, ability);
            
            messagesSystem.SendMessage(MessagesBoxTag,
                MainUpdaterUpdateTime.Update,
                this,
                new UnitControllerInstruction {
                    Unit = selectedActor,
                    Executable = instruction 
                });
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
