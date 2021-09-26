using Base;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class StartSelectingMultipleUnits : CoroutineProcess
    {
        enum State
        {
            Undetermined,
            MouseMovedAway,
            MouseReleased,
        }

        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectionInitialPositionTag;

        protected override IEnumerator<object> Crt()
        {
            context.PutItem(SelectionInitialPositionTag, null);

            Vector3 getFloorPoint()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.First(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));
                Vector3 floorPoint = floorHit.point;
                return floorPoint;
            }

            var game = Game.Instance;
            Vector3 initialFloorHit = Vector3.zero;
            IEnumerator<State> detectCrt()
            {
                while (Input.GetAxis("Select") <= 0)  
                {
                    yield return State.Undetermined;
                }

                Vector3 screenPos = Input.mousePosition;
                initialFloorHit = getFloorPoint();

                while (true)
                {
                    if (Input.GetAxis("Select") <= 0)
                    {
                        yield return State.MouseReleased;
                        continue;
                    }

                    Vector2 offset = Input.mousePosition - screenPos;
                    if (offset.sqrMagnitude > 25)
                    {
                        yield return State.MouseMovedAway;
                        continue;
                    }

                    yield return State.Undetermined;
                }
            }

            while (true)
            {
                context.PutItem(SelectionInitialPositionTag, null);
                var detect = detectCrt();
                detect.MoveNext();
                while (detect.Current == State.Undetermined)
                {
                    yield return null;
                    detect.MoveNext();
                }

                if (detect.Current == State.MouseMovedAway)
                {
                    context.PutItem(SelectionInitialPositionTag, initialFloorHit);
                    yield break;
                }
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
