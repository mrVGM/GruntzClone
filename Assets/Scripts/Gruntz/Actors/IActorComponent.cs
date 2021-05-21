using UnityEngine;

namespace Gruntz.Actors
{
    public interface IActorComponent
    {
        void Init();
        void DeInit();
    }
}
