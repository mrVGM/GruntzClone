using System;
using Base;
using Base.MessagesSystem;
using Gruntz.Status;
using Utils;

namespace Gruntz
{
    [Serializable]
    public class ArrowStatusData : StatusData
    {
        public SerializedVector3 Destination;
        public SerializedVector3 Anchor;
    }
}
