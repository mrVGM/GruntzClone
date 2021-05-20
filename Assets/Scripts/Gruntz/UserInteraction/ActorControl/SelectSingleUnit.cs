using Base;
using Gruntz.Actors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class SelectSingleUnit : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;

        protected override IEnumerator<object> Crt()
        {
            Actor getActor()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var actorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == UnityLayers.UnitSelection);
                if (actorHit.collider == null)
                {
                    return null;
                }
                var actorComponent = actorHit.collider.GetComponentInParent<ActorComponent>();
                var actorManager = ActorManager.GetActorManagerFromContext();
                var actor = actorManager.Actors.FirstOrDefault(x => x.ActorComponent == actorComponent);
                return actor;
            }

            var game = Game.Instance;
            while (true) {
                while (Input.GetAxis("Select") <= 0)
                {
                    yield return null;
                }
                var initialActor = getActor();
                while (Input.GetAxis("Select") > 0)
                {
                    yield return null;
                }
                if (initialActor == null)
                {
                    continue;
                }
                var finalActor = getActor();
                if (initialActor == finalActor)
                {
                    var list = new List<Actor>();
                    list.Add(initialActor);
                    context.PutItem(SelectedActorsTag, list);
                    yield break;
                }
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
