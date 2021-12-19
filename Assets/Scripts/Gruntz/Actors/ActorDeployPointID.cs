using System;
using UnityEngine;

namespace Gruntz.Actors
{
    public class ActorDeployPointID : MonoBehaviour
    {
        public string ID = Guid.NewGuid().ToString();
    }
}
