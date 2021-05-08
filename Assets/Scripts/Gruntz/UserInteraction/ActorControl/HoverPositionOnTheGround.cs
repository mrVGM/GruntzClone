using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class HoverPositionOnTheGround : CoroutineProcess
    {
        public MessagesBoxTagDef HitResultsMessageTag;
        public GameObject GroundSelectionMarker;

        protected override IEnumerator<object> Crt()
        {
            GroundSelectionMarker.SetActive(true);

            var game = Game.Instance;

            var messageSystemTag = game.DefRepositoryDef.AllDefs.OfType<MessagesSystemDef>().FirstOrDefault();
            var messagesSystem = game.Context.GetRuntimeObject(messageSystemTag) as MessagesSystem;

            while (true)
            {
                var hits = messagesSystem.GetMessages(HitResultsMessageTag).FirstOrDefault().Data as IEnumerable<RaycastHit>;
                var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == UnityLayers.Floor);

                if (floorHit.collider == null)
                {
                    GroundSelectionMarker.SetActive(false);
                    yield return null;
                    continue;
                }

                Vector3 pos = floorHit.point;
                Vector3 center = 0.5f * Vector3.right + 0.5f * Vector3.forward;
                pos -= center;
                pos.x = Mathf.Round(pos.x);
                pos.z = Mathf.Round(pos.z);
                pos += center;
                GroundSelectionMarker.transform.position = pos;

                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            GroundSelectionMarker.SetActive(false);
            yield break;
        }
    }
}
