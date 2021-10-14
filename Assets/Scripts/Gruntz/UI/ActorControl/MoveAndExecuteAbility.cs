using Base;
using Base.Actors;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.AI;
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
            var simpleAI = selectedActor.GetComponent<SimpleAIComponent>();
            var abilitiesComponent = selectedActor.GetComponent<AbilitiesComponent>();
            simpleAI.CurrentAction = new MoveInMeleeRangeAndExecuteAbility(selectedActor, targetActor, abilitiesComponent.GetMainAbility());

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
