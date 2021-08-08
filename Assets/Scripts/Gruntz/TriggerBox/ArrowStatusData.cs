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
        public DefRef<MessagesBoxTagDef> PreviousUnitControllerChannel;
        public SerializedVector3 Destination;
    }
}
