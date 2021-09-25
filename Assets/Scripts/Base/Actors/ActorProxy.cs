using System.Linq;
using UnityEngine;

namespace Base.Actors
{
    public class ActorProxy : MonoBehaviour
    {
        public ActorComponent ActorComponent;
        public Actor Actor
        {
            get
            {
                var actorManager = ActorManager.GetActorManagerFromContext();
                return actorManager.Actors.FirstOrDefault(x => x.ActorComponent == ActorComponent);
            }
        }
    }
}
