using Base.Status;
using System;

namespace Gruntz.Statuses
{
    [Serializable]
    public class OverrideActorControllerStatusData : StatusData
    {
        public string AssociatedStatusId;
    }
}
