using Base;
using LevelResults;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelection.UserInteraction
{
    public class LevelSelectionTooltip : MonoBehaviour
    {
        public Text LevelName;
        public Button StartLevel;
        public TagDef FinishedLevelsSaveTagDef;

        private void PlaceTooltip(Site site)
        {
            var game = Game.Instance;
            Vector3 tooltipScreenPos = game.Camera.WorldToScreenPoint(site.TooltipWorldPosition.position);
            var tooltip = GetComponent<RectTransform>();
            tooltip.position = tooltipScreenPos;

            switch (site.TooltipRelativePos)
            {
                case Site.TooltipPos.TopLeft:
                    tooltip.pivot = new Vector2(0, 1);
                    break;
                case Site.TooltipPos.TopRight:
                    tooltip.pivot = new Vector2(1, 1);
                    break;
                case Site.TooltipPos.BottomRight:
                    tooltip.pivot = new Vector2(1, 0);
                    break;
                case Site.TooltipPos.BottomLeft:
                    tooltip.pivot = new Vector2(0, 0);
                    break;
            }
        }

        public void ShowHoverTooltip(Site site, bool visible)
        {
            StopAllCoroutines();
            if (!visible) {
                if (!gameObject.activeSelf) {
                    return;
                }
                IEnumerator disableCrt()
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    gameObject.SetActive(false);
                }
                StartCoroutine(disableCrt());
                return;
            }

            var levelProvider = site as ILevelProvider;
            if (levelProvider == null) {
                return;
            }

            var level = levelProvider.LevelDef;
            gameObject.SetActive(true);
            LevelName.text = level.Name;
            StartLevel.gameObject.SetActive(false);
            PlaceTooltip(site);
        }

        public void ShowOnSiteTooltip(Site site, bool visible)
        {
            StopAllCoroutines();
            if (!visible) {
                if (!gameObject.activeSelf) {
                    return;
                }
                IEnumerator disableCrt()
                {
                    yield return null;
                    yield return null;
                    yield return null;
                    gameObject.SetActive(false);
                }
                StartCoroutine(disableCrt());
                return;
            }

            var levelProvider = site as ILevelProvider;
            if (levelProvider == null) {
                return;
            }
            var level = levelProvider.LevelDef;

            gameObject.SetActive(true);

            var levelProgress = LevelProgressInfo.GetLevelProgressInfoFromContext();
            levelProgress.CurrentLevel = level;
            LevelName.text = level.Name;
            StartLevel.gameObject.SetActive(true);
            StartLevel.onClick.RemoveAllListeners();

            var game = Game.Instance;
            StartLevel.onClick.AddListener(() => {
                game.SavesManager.CreateSave(FinishedLevelsSaveTagDef);
                game.LoadLevel(level, () => { });
            });

            PlaceTooltip(site);
        }
    }
}
