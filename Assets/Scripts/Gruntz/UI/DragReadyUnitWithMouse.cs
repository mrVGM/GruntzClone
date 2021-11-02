using Base;
using System.Collections.Generic;
using UnityEngine;
using Base.UI;
using System.Linq;
using Base.Actors;
using Base.Status;
using Gruntz.EquipmentVisuals;

namespace Gruntz.UI
{
    public class DragReadyUnitWithMouse : CoroutineProcess
    {
        public Transform DraggingHandle;
        public GameObject DraggedActor;

        public ProcessContextTagDef HitResultsTag;
        public StatusDef SpawnPointStatusDef;

        public Vector3 VerticalOffset = Vector3.up;

        protected override IEnumerator<object> Crt()
        {

            void enableLagging(bool enabled)
            {
                var laggingPointTargets = DraggingHandle.GetComponentsInChildren<LaggingPointTarget>();
                foreach (var lp in laggingPointTargets) {
                    lp.LaggingEnabled = enabled;
                }
            }

            void updateHandleTransform()
            {
                enableLagging(true);
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(Utils.UnityLayers.Floor));
                Vector3 floorPoint = floorHit.point;
                DraggingHandle.transform.position = floorPoint + VerticalOffset;

                var spawnPoint = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(Utils.UnityLayers.ActorGeneral));
                if (spawnPoint.Equals(default(RaycastHit))) {
                    return;
                }
                var spawnPointActor = spawnPoint.collider.GetComponent<ActorProxy>().Actor;
                var statusComponent = spawnPointActor.GetComponent<StatusComponent>();
                if (statusComponent.GetStatus(SpawnPointStatusDef) == null) {
                    return;
                }

                enableLagging(false);
                DraggingHandle.transform.position = spawnPointActor.Pos;
            }

            updateHandleTransform();
            enableLagging(false);
            yield return null;
            DraggedActor.SetActive(true);

            while (true) {
                updateHandleTransform();
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            DraggedActor.SetActive(false);
            yield break;
        }
    }
}
