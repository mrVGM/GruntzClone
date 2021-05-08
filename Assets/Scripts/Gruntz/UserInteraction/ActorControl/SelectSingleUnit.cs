using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class SelectSingleUnit : CoroutineProcess
    {
        public MessagesBoxTagDef HitResultsMessageTag;
        public ProcessContextTagDef SelectedActorsTag;

        enum SelectActorState
        {
            Running,
            ActorSelectionSucceeded,
            ActorSelectionFailed
        }
        private IEnumerator<SelectActorState> SelectUnit(MessagesSystem messagesSystem)
        {
            Actor GetHoveredActor()
            {
                var hits = messagesSystem.GetMessages(HitResultsMessageTag).First().Data as IEnumerable<RaycastHit>;
                var unitHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == UnityLayers.UnitSelection);

                if (unitHit.collider == null)
                {
                    return null;
                }

                var actor = unitHit.collider.GetComponentInParent<Actor>();
                return actor;
            }

            while (Input.GetAxis("Select") <= 0)
            {
                yield return SelectActorState.Running;
            }

            var actor = GetHoveredActor();
            
            while (Input.GetAxis("Select") > 0)
            {
                yield return SelectActorState.Running;
            }

            if (actor == null)
            {
                Debug.Log($"Selected actor: null");
                yield return SelectActorState.ActorSelectionFailed;
                yield break;
            }

            var hoveredActor = GetHoveredActor();
            if (hoveredActor == actor)
            {
                var selectedActors = new List<Actor>();
                selectedActors.Add(actor);
                context.PutItem(SelectedActorsTag, selectedActors);
                Debug.Log($"Selected actor: {actor}", actor);
                yield return SelectActorState.ActorSelectionSucceeded;
            }
            else
            {
                Debug.Log($"Selected actor: null");
                yield return SelectActorState.ActorSelectionFailed;
            }
        }

        protected override IEnumerator<object> Crt()
        {
            var game = Game.Instance;
            var messageSystemTag = game.DefRepositoryDef.AllDefs.OfType<MessagesSystemDef>().FirstOrDefault();
            var messagesSystem = game.Context.GetRuntimeObject(messageSystemTag) as MessagesSystem;

            while (true)
            {
                var actorSelectCrt = SelectUnit(messagesSystem);
                actorSelectCrt.MoveNext();

                SelectActorState state = actorSelectCrt.Current;
                while (state == SelectActorState.Running)
                {
                    yield return null;
                    actorSelectCrt.MoveNext();
                    state = actorSelectCrt.Current;
                }
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
