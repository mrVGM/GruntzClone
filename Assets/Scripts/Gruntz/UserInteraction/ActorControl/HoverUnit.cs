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
        public ProcessContextTagDef HitResultsTag;
        public GameObject UnitSelectionMarker;
        public ProcessContextTagDef DoingSelectionTagDef;

        public void DoUpdate(MessagesSystem messagesSystem)
        {
            bool doingSelection = false;
            object contextItem = context.GetItem(DoingSelectionTagDef);
            if (contextItem != null)
            {
                doingSelection = (bool) contextItem;
            }

            if (doingSelection)
            {
                UnitSelectionMarker.SetActive(false);
                return;
            }

            var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
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
