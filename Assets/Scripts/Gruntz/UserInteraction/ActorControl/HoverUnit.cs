using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class HoverUnit : Process
    {
        public MessagesBoxTagDef HitResultsMessageTag;
        public GameObject UnitSelectionMarker;

        bool isRunning = false;

        public override bool IsFinished => isTermninated && !isRunning;

        public override void DoUpdate()
        {
            if (!isTermninated)
            {
                isRunning = true;
            }
            else
            {
                UnitSelectionMarker.SetActive(false);
                isRunning = false;
                return;
            }

            var game = Game.Instance;

            var messageSystemTag = game.DefRepositoryDef.AllDefs.OfType<MessagesSystemDef>().FirstOrDefault();
            var messagesSystem = game.Context.GetRuntimeObject(messageSystemTag) as MessagesSystem;
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
    }
}
