using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.AI;
using Gruntz.Highlight;
using Gruntz.UnitController;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class HighlightAbilityTarget : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;

        public Actor CurrentTarget;
        protected override IEnumerator<object> Crt()
        {
            while (true)
            {
                var selected = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
                if (selected == null || selected.Count() != 1) {
                    break;
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

                var selectedActor = selected.First();
                var abilitiesComponent = selectedActor.GetComponent<AbilitiesComponent>();

                var ability = abilitiesComponent.GetMainAbility();
                var targetActor = getTargetActor();

                if (targetActor == null || !abilitiesComponent.CanExecuteOn(ability, targetActor)) {
                    break;
                }
                if (CurrentTarget != null && CurrentTarget.IsInPlay) {
                    var highlight = CurrentTarget.GetComponent<HighlightComponent>();
                    if (highlight != null) {
                        highlight.SetHighlight(false);
                    }
                }
                var highligComponent = targetActor.GetComponent<HighlightComponent>();
                highligComponent.SetHighlight(true);
                CurrentTarget = targetActor;
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            if (CurrentTarget != null && CurrentTarget.IsInPlay) {
                var highlight = CurrentTarget.GetComponent<HighlightComponent>();
                if (highlight != null) {
                    highlight.SetHighlight(false);
                }
            }
            yield break;
        }
    }
}
