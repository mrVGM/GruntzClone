using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Gruntz.Actors;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class HoverUnit : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;
        public GameObject UnitSelectionMarker;

        public void UpdateMarkers()
        {
            UnitSelectionMarker.transform.position = 1000 * Vector3.down;

            var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
            var unitHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.UnitSelection));
            if (unitHit.collider == null)
            {
                return;
            }

            var actorComponent = unitHit.collider.GetComponentInParent<ActorComponent>();
            var actorManager = ActorManager.GetActorManagerFromContext();
            var actor = actorManager.Actors.FirstOrDefault(x => x.ActorComponent == actorComponent);

            var selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selectedActors != null && selectedActors.Contains(actor))
            {
                return;
            }
            UnitSelectionMarker.transform.position = actor.Pos;
        }

        protected override IEnumerator<object> Crt()
        {
            while (true)
            {
                UpdateMarkers();
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            UnitSelectionMarker.transform.position = 1000 * Vector3.down;
            yield break;
        }
    }
}
