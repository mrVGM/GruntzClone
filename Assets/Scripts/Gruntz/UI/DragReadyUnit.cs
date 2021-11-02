using Base;
using System.Collections.Generic;
using UnityEngine;
using Base.UI;
using UnityEngine.EventSystems;
using System.Linq;
using Base.Navigation;
using Gruntz.Actors;
using Gruntz.Gameplay;

namespace Gruntz.UI
{
    public class DragReadyUnit : CoroutineProcess
    {
        public ActorTemplateDef ActorTemplate;
        public ProcessContextTagDef StartedDragging;
        public ProcessContextTagDef HitResultsTag;

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
            var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(Utils.UnityLayers.Floor));
            var navigation = Navigation.GetNavigationFromContext();
            var map = navigation.Map;
            Vector3 pos = map.SnapPosition(floorHit.point);

            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
            gameplayManager.HandleGameplayEvent(new SpawnActorGameplayEvent { Template = ActorTemplate, Pos = pos });
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
