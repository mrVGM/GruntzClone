using Base;
using Base.UI;
using LevelResults;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelection.UserInteraction
{
    public class ShowTooltip : CoroutineProcess
    {
        public RectTransform Tooltip;
        public Text LevelName;
        public Button StartLevel;
        public TagDef FinishedLevelsSaveTagDef;

        protected override IEnumerator<object> Crt()
        {
            var levelSelectionMap = LevelSelectionMap.GetLevelSelectionMapFromContext();
            var unit = levelSelectionMap.Unit;

            while (unit.IsWalking) {
                yield return null;
            }

            var site = unit.CurrentSite;
            var levelProgress = LevelProgressInfo.GetLevelProgressInfoFromContext();
            levelProgress.CurrentLevel = site.LevelDef;
            LevelName.text = site.LevelDef.Name;
            StartLevel.onClick.RemoveAllListeners();
            
            var game = Game.Instance;
            StartLevel.onClick.AddListener(() => {
                game.SavesManager.CreateSave(FinishedLevelsSaveTagDef);
                game.LoadLevel(site.LevelDef, () => { });
            });

            Vector3 tooltipScreenPos = game.Camera.WorldToScreenPoint(site.TooltipWorldPosition.position);
            Tooltip.position = tooltipScreenPos;

            switch (site.TooltipRelativePos) {
                case Site.TooltipPos.TopLeft:
                    Tooltip.pivot = new Vector2(0, 1);
                    break;
                case Site.TooltipPos.TopRight:
                    Tooltip.pivot = new Vector2(1, 1);
                    break;
                case Site.TooltipPos.BottomRight:
                    Tooltip.pivot = new Vector2(1, 0);
                    break;
                case Site.TooltipPos.BottomLeft:
                    Tooltip.pivot = new Vector2(0, 0);
                    break;
            }

            Tooltip.gameObject.SetActive(true);
            while (!unit.IsWalking) {
                yield return null;
            }
            Tooltip.gameObject.SetActive(false);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            Tooltip.gameObject.SetActive(false);
            yield break;
        }
    }
}
