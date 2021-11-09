using Base.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace LevelSelection.UserInteraction
{
    public class MoveUnit : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        protected override IEnumerator<object> Crt()
        {
            while (Input.GetAxis("Select") > 0) {
                yield return null;
            }

            while (Input.GetAxis("Select") <= 0) {
                yield return null;
            }

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

            var site = getPointedSite();
            if (site == null) {
                yield break;
            }

            while (Input.GetAxis("Select") > 0) {
                yield return null;
            }

            var siteAfterRelease = getPointedSite();
            if (site == siteAfterRelease) {
                var levelSelectionMap = LevelSelectionMap.GetLevelSelectionMapFromContext();
                levelSelectionMap.Unit.TargetSite = site;
            }

            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
