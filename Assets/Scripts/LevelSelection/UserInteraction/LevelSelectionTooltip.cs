using Base;
using LevelResults;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelection.UserInteraction
{
    public class LevelSelectionTooltip : MonoBehaviour
    {
        public Text LevelName;
        public Button StartLevel;
        public TagDef FinishedLevelsSaveTagDef;
        public GameObject AreaButtonsContainer;
        public Button AreaButtonTemplate;

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
            AreaButtonsContainer.SetActive(false);
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

            LevelName.gameObject.SetActive(true);
            StartLevel.gameObject.SetActive(true);

            var level = levelProvider.LevelDef;
            gameObject.SetActive(true);
            LevelName.text = level.Name;
            StartLevel.gameObject.SetActive(false);
            PlaceTooltip(site);
        }

        public void ShowOnSiteTooltip(Site site, bool visible)
        {
            AreaButtonsContainer.SetActive(false);
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

            gameObject.SetActive(true);
            LevelName.gameObject.SetActive(true);
            var levelProvider = site as ILevelProvider;
            if (levelProvider == null) {
                ShowAreas();
                PlaceTooltip(site);
                return;
            }
            var level = levelProvider.LevelDef;

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

        public void ShowAreas()
        {
            var levelSelectionMap = LevelSelectionMap.GetLevelSelectionMapFromContext();
            var levelProgress = LevelProgressInfo.GetLevelProgressInfoFromContext();

            var activeAreas = levelSelectionMap.Areas.Where(x => {
                var levelProviderSites = x.Sites.OfType<ILevelProvider>();
                return levelProviderSites.Any(y => levelProgress.IsLevelUnlocked(y.LevelDef));
            });
            var currentArea = levelSelectionMap.Areas.FirstOrDefault(x => {
                var currentSite = levelSelectionMap.Unit.CurrentSite;
                return x.Sites.Contains(currentSite);
            });

            StartLevel.gameObject.SetActive(false);
            LevelName.gameObject.SetActive(false);

            AreaButtonsContainer.SetActive(true);
            int index = 0;
            foreach (var area in activeAreas) {
                if (area == currentArea) {
                    continue;
                }

                while (index >= AreaButtonsContainer.transform.childCount) {
                    Instantiate(AreaButtonTemplate, AreaButtonsContainer.transform);
                }

                var button = AreaButtonsContainer.transform.GetChild(index++).GetComponent<Button>();
                var text = button.GetComponentInChildren<Text>();
                text.text = area.Label;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    levelSelectionMap.TeleportToArea(area);
                });
                button.gameObject.SetActive(true);
            }

            for (int i = index; i < AreaButtonsContainer.transform.childCount; ++i) {
                AreaButtonsContainer.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
