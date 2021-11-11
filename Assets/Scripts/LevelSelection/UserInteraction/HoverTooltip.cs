using Base.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace LevelSelection.UserInteraction
{
    public class HoverTooltip : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef HoveredSite;
        public LevelSelectionTooltip Tooltip;

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

            var levelSelectionMap = LevelSelectionMap.GetLevelSelectionMapFromContext();
            var unit = levelSelectionMap.Unit;

            var site = context.GetItem(HoveredSite) as Site;
            if (site == null) {
                yield break;
            }

            Tooltip.ShowHoverTooltip(site, true);
            while (site == getPointedSite() && unit.CurrentSite != site) {
                yield return null;
            }
            Tooltip.ShowHoverTooltip(site, false);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
