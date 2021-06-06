using System;
using Base;
using UnityEngine;
using Utils;

namespace Gruntz.Navigation
{
    [Serializable]
    public class NavAgentData : ISerializedObjectData
    {
        public SerializedVector3 InitialPosition;
        public SerializedVector3 Target;
        public float Speed;
    }
}
