using Base;
using System.Collections.Generic;
using UnityEngine;
using Base.UI;

namespace Gruntz.UI
{
    public class UpdateReadyUnits : CoroutineProcess
    {
        public RectTransform ReadyUnitsContainer;
        public ReadyUnit ReadyUnitTemplate;
        public ProcessContextTagDef StartedDragging;

        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var collectedMaterilaManager = CollectedMaterialManager.CollectedMaterialManager.GetCollectedMaterialManager();

            ReadyUnit drag = null;

            while (true) {
                context.PutItem(StartedDragging, null);
                if (drag != null) {
                    context.PutItem(StartedDragging, drag);
                    yield break;
                }
                drag = null;

                int iconsNeeded = Mathf.CeilToInt((float)collectedMaterilaManager.MaterialPiecesCollected / collectedMaterilaManager.CollectedMaterialManagerDef.MaterialNeededForFullUnit);
                for (int i = 0; i < iconsNeeded; ++i) {
                    if (ReadyUnitsContainer.childCount < iconsNeeded) {
                        Instantiate(ReadyUnitTemplate, ReadyUnitsContainer);
                    }
                }

                for (int i = 0; i < iconsNeeded; ++i) {
                    int materialLeft = collectedMaterilaManager.MaterialPiecesCollected - i * collectedMaterilaManager.CollectedMaterialManagerDef.MaterialNeededForFullUnit;
                    float fill = (float)materialLeft / collectedMaterilaManager.CollectedMaterialManagerDef.MaterialNeededForFullUnit;
                    fill = Mathf.Clamp01(fill);
                    var cur = ReadyUnitsContainer.GetChild(i).GetComponent<ReadyUnit>();
                    cur.Filling.fillAmount = fill;
                    cur.PointerDown = null;
                    cur.gameObject.SetActive(true);
                    if (fill >= 1) {
                        cur.PointerDown = () => {
                            drag = cur;
                        };
                    }
                }

                for (int i = iconsNeeded; i < ReadyUnitsContainer.childCount; ++i) {
                    var cur = ReadyUnitsContainer.GetChild(i);
                    cur.gameObject.SetActive(false);
                }
                
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            for (int i = 0; i < ReadyUnitsContainer.childCount; ++i) {
                var child = ReadyUnitsContainer.GetChild(i);
                child.gameObject.SetActive(false);
            }

            yield break;
        }
    }
}
