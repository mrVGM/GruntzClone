using System;
using System.Collections.Generic;

namespace Base.Actors
{
    [Serializable]
    public class ActorManagerData : ISerializedObjectData
    {
        public List<ActorData> ActorDatas = new List<ActorData>();
    }
}
