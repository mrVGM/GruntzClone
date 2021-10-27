using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.Navigation;
using Base.UI;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;
using static Base.Navigation.NavAgent;

namespace Gruntz.UI.ActorControl
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

            if (ActorControlUtils.CanStepToPosition(selected.Select(x => x.GetComponent<NavAgent>()), pos)) {
                var marker = Instantiate(Marker);
                marker.transform.position = pos;

                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                var moveToPositionIntruction = new MoveToPosition { Target = new SimpleNavTarget { Target = pos } };


                foreach (var actor in selected)
                {
                    messagesSystem.SendMessage(
                        MessagesBoxTag,
                        MainUpdaterUpdateTime.Update,
                        this,
                        new UnitControllerInstruction
                        {
                            Unit = actor,
                            Executable = moveToPositionIntruction,
                        });
                }
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
