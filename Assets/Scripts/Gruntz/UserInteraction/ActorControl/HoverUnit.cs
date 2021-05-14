using System.Collections.Generic;
using System.Linq;
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
            var unitHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == UnityLayers.UnitSelection);
            if (unitHit.collider == null)
            {
                return;
            }

            var actor = unitHit.collider.GetComponentInParent<Actor>();

            var selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selectedActors != null && selectedActors.Contains(actor))
            {
                return;
            }
            UnitSelectionMarker.transform.position = actor.transform.position;
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
