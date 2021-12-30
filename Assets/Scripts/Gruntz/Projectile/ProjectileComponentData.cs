using Base;
using System;
using System.Collections.Generic;

namespace Gruntz.Projectile
{
    [Serializable]
    public class ProjectileComponentData : ISerializedObjectData
    {
        public float LifeTime;
        public SerializedVector3 StartPoint;
        public SerializedVector3 EndPoint;
    }
}
