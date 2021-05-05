using System.Linq;
using Base;
using Base.MessagesSystem;
using UnityEngine;

namespace Gruntz.UserInteraction.ActorControl
{
    public class PointerCast : Process
    {
        public MessagesBoxTagDef HitResultsMessageTag;

        private RaycastHit[] hits = new RaycastHit[100];
        public override bool IsFinished => isTermninated;

        public override void DoUpdate()
        {
            var cam = Game.Instance.Camera;
            var cursorRay = cam.ScreenPointToRay(Input.mousePosition);

            int numOfHits = Physics.RaycastNonAlloc(cursorRay, hits);
            var realHits = hits.Take(numOfHits);

            var game = Game.Instance;
            var messageSystemTag = game.DefRepositoryDef.AllDefs.OfType<MessagesSystemDef>().FirstOrDefault();
            var messagesSystem = game.Context.GetRuntimeObject(messageSystemTag) as MessagesSystem;
            messagesSystem.SendMessage(HitResultsMessageTag, this, realHits);
        }
    }
}
