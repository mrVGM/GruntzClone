using Base.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace LevelSelection.UserInteraction
{
    public class HoverTooltipStart : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef HoveredSite;
        public float Delay = 0.2f;

        protected override IEnumerator<object> Crt()
        {
            Site getPointedSite()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));

                if (floorHit.Equals(default(RaycastHit)))
                {
                    return null;
                }

                var site = floorHit.collider.GetComponentInParent<Site>();
                return site;
            }

            var unit = LevelSelectionMap.GetLevelSelectionMapFromContext().Unit;
            context.PutItem(HoveredSite, null);

            var site = getPointedSite();
            while (site == null || site == unit.CurrentSite) {
                yield return null;
                site = getPointedSite();
            }

            float startTime = Time.time;
            while (Time.time - startTime < Delay) {
                if (site != getPointedSite() || site == unit.CurrentSite) {
                    yield break;
                }
                yield return null;
            }

            context.PutItem(HoveredSite, site);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
