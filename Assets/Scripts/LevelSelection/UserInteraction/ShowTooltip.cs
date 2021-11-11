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
            Tooltip.ShowStartLevelTooltip(Site, true);
            while (!unit.IsWalking) {
                yield return null;
            }
            Tooltip.ShowStartLevelTooltip(Site, false);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            Tooltip.ShowStartLevelTooltip(Site, false);
            yield break;
        }
    }
}
