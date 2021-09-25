using Base;
using Base.Actors;
using Base.MessagesSystem;
using Gruntz.UserInteraction.UnitController;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class MoveUnitsToPosition : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;
        public MessagesBoxTagDef MessagesBoxTag;

        public GameObject Marker;

        protected override IEnumerator<object> Crt()
        {
            var selected = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selected == null || !selected.Any())
            {
                yield break;
            }

            Vector3 getFloorPoint()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.First(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));
                Vector3 floorPoint = floorHit.point;
                return floorPoint;
            }

            var game = Game.Instance;

            while (Input.GetAxis("Move") <= 0)
            {
                yield return null;
            }

            var pos = getFloorPoint();
            Vector3 center = 0.5f * Vector3.right + 0.5f * Vector3.forward;
            pos -= center;
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            pos += center;

            var marker = Instantiate(Marker);
            marker.transform.position = pos;

            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
            var moveToPositionIntruction = new MoveToPosition { Position = pos };
            foreach (var actor in selected)
            {
                messagesSystem.SendMessage(MessagesBoxTag, this, new UnitControllerInstruction
                {
                    Unit = actor,
                    Executable = moveToPositionIntruction,
                });
            }

            while (Input.GetAxis("Move") > 0)
            {
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
