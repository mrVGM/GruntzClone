using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using UnityEngine;

namespace Gruntz.UserInteraction.ActorControl
{
    public class PointerCast : CoroutineProcess
    {
        public MessagesBoxTagDef HitResultsMessageTag;

        private RaycastHit[] hits = new RaycastHit[100];

        public void DoUpdate(MessagesSystem messagesSystem)
        {
            var cam = Game.Instance.Camera;
            var cursorRay = cam.ScreenPointToRay(Input.mousePosition);

            int numOfHits = Physics.RaycastNonAlloc(cursorRay, hits);
            var realHits = hits.Take(numOfHits);

            messagesSystem.SendMessage(HitResultsMessageTag, this, realHits);
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
            yield break;
        }
    }
}
