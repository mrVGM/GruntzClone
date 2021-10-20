using Base.Actors;
using UnityEngine;

namespace Gruntz.Actors
{
    public class ActorDeployPoint : MonoBehaviour
    {
        public ActorComponent ActorComponent
        {
            get
            {
                for (int i = 0; i < transform.childCount; ++i) {
                    var actorComponent = transform.GetChild(i).GetComponent<ActorComponent>();
                    if (actorComponent != null) {
                        return actorComponent;
                    }
                }
                return null;
            }
        }
        public ActorTemplateDef ActorTempleteDef;
    }
}
