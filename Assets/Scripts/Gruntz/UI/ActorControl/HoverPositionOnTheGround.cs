using Base;
using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class HoverPositionOnTheGround : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;
        public GameObject GroundSelectionMarker;

        void UpdateMarker()
        {
            GroundSelectionMarker.transform.position = 1000 * Vector3.down;

            var selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selectedActors == null || !selectedActors.Any()) {
                return;
            }
            var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
            hits = hits.Where(x => !x.collider.isTrigger);
            if (hits.Count() > 1) {
                return;
            }
            var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));

            if (floorHit.collider == null) {
                return;
            }

            Vector3 pos = floorHit.point;
            Vector3 center = 0.5f * Vector3.right + 0.5f * Vector3.forward;
            pos -= center;
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            pos += center;

            var game = Game.Instance;
            var actorManagerDef = game.DefRepositoryDef.AllDefs.OfType<ActorManagerDef>().FirstOrDefault();
            var actorManager = game.Context.GetRuntimeObject(actorManagerDef) as ActorManager;

            var actors = actorManager.Actors;
            if (actors.Any(x => x.GetComponent<Base.Navigation.NavAgent>() != null && (x.Pos - pos).sqrMagnitude <= 0.25f)) {
                return;
            }

            GroundSelectionMarker.transform.position = pos;
        }

        protected override IEnumerator<object> Crt()
        {
            GroundSelectionMarker.SetActive(true);

            while (true) {
                UpdateMarker();
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            GroundSelectionMarker.transform.position = 1000 * Vector3.down;
            yield break;
        }
    }
}
