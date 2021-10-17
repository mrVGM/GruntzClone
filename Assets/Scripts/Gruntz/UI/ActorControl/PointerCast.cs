using System.Collections.Generic;
using System.Linq;
using Base;
using Base.UI;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class PointerCast : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;

        private RaycastHit[] hits = new RaycastHit[100];

        private void UpdateHits()
        {
            var cam = Game.Instance.Camera;
            var cursorRay = cam.ScreenPointToRay(Input.mousePosition);

            int numOfHits = Physics.RaycastNonAlloc(cursorRay, hits);
            
            var realHits = hits.Take(numOfHits).Where(x => x.collider.gameObject.layer != LayerMask.NameToLayer(UnityLayers.Ignored));

            context.PutItem(HitResultsTag, realHits);
        }

        protected override IEnumerator<object> Crt()
        {
            while (true)
            {
                UpdateHits();
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
