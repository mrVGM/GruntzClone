using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.UserInteraction.ActorControl
{
    public class HighlightSelectedUnits : CoroutineProcess
    {
        public ProcessContextTagDef SelectedActorsTag;
        public Transform UnitSelectionMarkersContainer;

        GameObject GetSelectionMarker()
        {
            int count = UnitSelectionMarkersContainer.childCount;
            for (int i = 0; i < count; ++i)
            {
                var cur = UnitSelectionMarkersContainer.GetChild(i);
                if (cur.transform.position.y <= -1000.0f)
                {
                    return cur.gameObject;
                }
            }

            var newMarker = Instantiate(UnitSelectionMarkersContainer.GetChild(0).gameObject, UnitSelectionMarkersContainer);
            return newMarker;
        }

        void DisableMarkers()
        {
            for (int i = 0; i < UnitSelectionMarkersContainer.childCount; ++i)
            {
                var cur = UnitSelectionMarkersContainer.GetChild(i);
                cur.position = 1000 * Vector3.down;
            }
        }

        void UpdateMarkers()
        {
            DisableMarkers();
            var selected = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selected == null)
            {
                return;
            }

            foreach (var actor in selected)
            {
                var go = GetSelectionMarker();
                go.transform.position = actor.Pos;
            }
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
            DisableMarkers();
            yield break;
        }
    }
}
