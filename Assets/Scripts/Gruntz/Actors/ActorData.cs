using System;
using Base;
using UnityEngine;

namespace Gruntz.Actors
{
    [Serializable]
    public class ActorData : ISerializedObjectData
    {
        [Serializable]
        public class Components
        {
            public ActorComponentDef Component;
            public ISerializedObjectData Data;
        }

        public ActorComponent ActorPrefab;
        public Components[] ActorComponents;
    }
}
