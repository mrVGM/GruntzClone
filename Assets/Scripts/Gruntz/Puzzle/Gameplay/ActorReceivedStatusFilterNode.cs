using Gruntz.Status;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Gruntz.Status.StatusComponent;

namespace Gruntz.Puzzle.Gameplay
{
    public class ActorReceivedStatusFilterNode : MonoBehaviour, IFilterNode
    {
        public StatusDef ActorStatus;
        public StatusDef StatusDef;
        public bool Filter(IEnumerable<GameplayEvent> gameplayEvents)
        {
            bool isInteresting(StatusGameplayEvent statusEvent)
            {
                if (statusEvent == null)
                {
                    return false;
                }
                var statusComponent = statusEvent.Actor.GetComponent<StatusComponent>();
                if (statusComponent.GetStatus(ActorStatus) == null)
                {
                    return false;
                }
                if (statusEvent.Status.StatusDef != StatusDef)
                {
                    return false;
                }
                return true;
            }

            return gameplayEvents.OfType<StatusGameplayEvent>().Any(isInteresting);
        }
    }
}
