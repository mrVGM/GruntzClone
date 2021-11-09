using Base.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace LevelSelection.UserInteraction
{
    public class HighlightSite : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public Site Highlighted;
        protected override IEnumerator<object> Crt()
        {
            Site getPointedSite()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));

                if (floorHit.Equals(default(RaycastHit))) {
                    return null;
                }

                var site = floorHit.collider.GetComponentInParent<Site>();
                return site;
            }

            while (true) {
                while (Highlighted == null) {
                    Highlighted = getPointedSite();
                    yield return null;
                }

                Highlighted.HighlightActive(true);

                while (Highlighted == getPointedSite()) {
                    yield return null;
                }
                Highlighted.HighlightActive(false);
                Highlighted = null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
