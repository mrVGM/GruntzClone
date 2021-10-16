using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.AI;
using Gruntz.UnitController;
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
            if (selected == null || selected.Count() != 1)
            {
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

            var selectedActor = selected.First();
            var abilitiesComponent = selectedActor.GetComponent<AbilitiesComponent>();

            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            var instruction = new MoveInMeleeRangeAndExecuteAbility(targetActor, abilitiesComponent.GetMainAbility());
            messagesSystem.SendMessage(MessagesBoxTag,
                MainUpdaterUpdateTime.Update,
                this,
                new UnitControllerInstruction {
                    Unit = selectedActor,
                    Executable = instruction 
                });

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