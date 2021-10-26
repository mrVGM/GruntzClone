using Base;
using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class SelectMultipleUnits : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;
        public ProcessContextTagDef InitialPositionTagDef;

        public RectTransform SelectionRectangle;
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

        void DisableSelectionMarkers()
        {
            int count = UnitSelectionMarkersContainer.childCount;
            for (int i = 0; i < count; ++i)
            {
                var cur = UnitSelectionMarkersContainer.GetChild(i);
                cur.transform.position = 1000 * Vector3.down;
            }
        }

        protected override IEnumerator<object> Crt()
        {
            Vector3 getFloorPoint()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.First(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));
                Vector3 floorPoint = floorHit.point;
                return floorPoint;
            }

            IEnumerable<Actor> getActorsInSelection(Vector3 point1, Vector3 point2)
            {
                Vector3 size = point2 - point1;
                size.x = Mathf.Abs(size.x);
                size.z = Mathf.Abs(size.z);
                size.y = 0.5f;

                var hits = Physics.OverlapBox(0.5f * (point1 + point2), 0.5f * size)
                    .Where(x => x.gameObject.layer == LayerMask.NameToLayer(UnityLayers.UnitSelection));

                var actorProxies = hits.Select(x => x.GetComponent<ActorProxy>()).ToList();
                var res = actorProxies.Select(x => x.Actor).Where(x => ActorControlUtils.CanSelectActor(x, Enumerable.Empty<Actor>()));
                return res;
            }

            var initialPositionObject = context.GetItem(InitialPositionTagDef);
            if (initialPositionObject == null)
            {
                yield break;
            }

            var game = Game.Instance;
            Vector3 firstPoint = (Vector3) initialPositionObject;
            Vector3 secondPoint = getFloorPoint();

            while (Input.GetAxis("Select") > 0)
            {
                secondPoint = getFloorPoint();
                SelectionRectangle.gameObject.SetActive(true);

                Vector3 scrrenPoint1 = game.Camera.WorldToScreenPoint(firstPoint);
                Vector3 scrrenPoint2 = game.Camera.WorldToScreenPoint(secondPoint);

                SelectionRectangle.transform.position = scrrenPoint1;
                Vector2 sizeDelta = scrrenPoint2 - scrrenPoint1;
                Vector2 sizeDeltaOrig = sizeDelta;
                sizeDelta.x = Mathf.Abs(sizeDelta.x);
                sizeDelta.y = Mathf.Abs(sizeDelta.y);
                SelectionRectangle.sizeDelta = sizeDelta;

                if (sizeDeltaOrig.x > 0 && sizeDeltaOrig.y > 0)
                {
                    SelectionRectangle.pivot = Vector2.zero;
                }
                if (sizeDeltaOrig.x <= 0 && sizeDeltaOrig.y > 0)
                {
                    SelectionRectangle.pivot = Vector2.right;
                }
                if (sizeDeltaOrig.x > 0 && sizeDeltaOrig.y <= 0)
                {
                    SelectionRectangle.pivot = Vector2.up;
                }
                if (sizeDeltaOrig.x <= 0 && sizeDeltaOrig.y <= 0)
                {
                    SelectionRectangle.pivot = Vector2.one;
                }

                var actorsInSelection = getActorsInSelection(firstPoint, secondPoint);
                DisableSelectionMarkers();
                foreach (var actor in actorsInSelection)
                {
                    var marker = GetSelectionMarker();
                    marker.transform.position = actor.Pos;
                }

                yield return null;
            }
            secondPoint = getFloorPoint();
            var actors = getActorsInSelection(firstPoint, secondPoint);
            context.PutItem(SelectedActorsTag, actors);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            SelectionRectangle.gameObject.SetActive(false);
            DisableSelectionMarkers();
            yield break;
        }
    }
}
