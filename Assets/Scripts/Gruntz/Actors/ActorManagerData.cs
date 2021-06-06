using Base;
using System;
using System.Collections.Generic;

namespace Gruntz.Actors
{
    [Serializable]
    public class ActorManagerData : ISerializedObjectData
    {
        public List<ActorData> ActorDatas = new List<ActorData>();
    }
}
