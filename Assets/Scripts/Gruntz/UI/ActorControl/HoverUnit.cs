using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.UI;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
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

            var actorProxy = unitHit.collider.GetComponent<ActorProxy>();
            var actor = actorProxy.Actor;

            var selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (!ActorControlUtils.CanSelectActor(actor, selectedActors))
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
