using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Base.Navigation
{
    [Serializable]
    public class SimplePositionComponentData : ISerializedObjectData
    {
        public SerializedVector3 Position = Vector3.zero;
    }
}
