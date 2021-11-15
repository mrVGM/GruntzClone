using Base.UI;
using System.Collections.Generic;

namespace LevelSelection.UserInteraction
{
    public class ShowTooltip : CoroutineProcess
    {
        public LevelSelectionTooltip Tooltip;
        public Site Site;

        protected override IEnumerator<object> Crt()
        {
            var levelSelectionMap = LevelSelectionMap.GetLevelSelectionMapFromContext();
            var unit = levelSelectionMap.Unit;

            while (unit.IsWalking) {
                yield return null;
            }

            Site = unit.CurrentSite;
            Tooltip.ShowOnSiteTooltip(Site, true);

            while (!unit.IsWalking && unit.CurrentSite == Site) {
                yield return null;
            }
            Tooltip.ShowOnSiteTooltip(Site, false);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            Tooltip.ShowOnSiteTooltip(Site, false);
            yield break;
        }
    }
}
