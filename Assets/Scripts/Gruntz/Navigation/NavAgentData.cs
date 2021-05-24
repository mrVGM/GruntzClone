using System;
using Base;
using UnityEngine;

namespace Gruntz.Navigation
{
    [Serializable]
    public class NavAgentData : ISerializedObjectData
    {
        public Vector3 InitialPosition;
        public Vector3 Target;
        public float Speed;
    }
}
