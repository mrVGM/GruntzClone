using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class HoverUnit : CoroutineProcess
    {
        public MessagesBoxTagDef HitResultsMessageTag;
        public GameObject UnitSelectionMarker;

        public void DoUpdate(MessagesSystem messagesSystem)
        {
            var hits = messagesSystem.GetMessages(HitResultsMessageTag).FirstOrDefault().Data as IEnumerable<RaycastHit>;
            var unitHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == UnityLayers.UnitSelection);
            if (unitHit.collider == null)
            {
                UnitSelectionMarker.SetActive(false);
                return;
            }

            var actor = unitHit.collider.GetComponentInParent<Actor>();
            UnitSelectionMarker.SetActive(true);
            UnitSelectionMarker.transform.position = actor.transform.position;
        }

        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var messageSystemTag = game.DefRepositoryDef.AllDefs.OfType<MessagesSystemDef>().FirstOrDefault();
            var messagesSystem = game.Context.GetRuntimeObject(messageSystemTag) as MessagesSystem;

            while (true)
            {
                DoUpdate(messagesSystem);
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            UnitSelectionMarker.SetActive(false);
            yield break;
        }
    }
}
