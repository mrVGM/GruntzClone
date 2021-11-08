using Base;
using System.Collections.Generic;
using UnityEngine;
using Base.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Base.Navigation;
using Gruntz.Actors;
using Gruntz.Gameplay;
using Base.Actors;
using Base.Status;
using Base.Gameplay;

namespace Gruntz.UI
{
    public class DragReadyUnit : CoroutineProcess
    {
        public ActorTemplateDef ActorTemplate;
        public ProcessContextTagDef StartedDragging;
        public ProcessContextTagDef HitResultsTag;
        public StatusDef SpawnPointStatusDef;

        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var collectedMaterilaManager = CollectedMaterialManager.CollectedMaterialManager.GetCollectedMaterialManager();

            while (context.GetItem(StartedDragging) == null) {
                yield return null;
            }

            while (Input.GetAxis("Select") > 0) {
                yield return null;
            }

            if (EventSystem.current.IsPointerOverGameObject()) {
                yield break;
            }

            var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
            var spawnPoint = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(Utils.UnityLayers.ActorGeneral));
            if (spawnPoint.Equals(default(RaycastHit))) {
                yield break;
            }
            var spawnPointActor = spawnPoint.collider.GetComponent<ActorProxy>().Actor;
            var statusComponent = spawnPointActor.GetComponent<StatusComponent>();
            if (statusComponent.GetStatus(SpawnPointStatusDef) == null) {
                yield break;
            }
            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
            gameplayManager.HandleGameplayEvent(new SpawnActorGameplayEvent { Template = ActorTemplate, Pos = spawnPointActor.Pos });
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
