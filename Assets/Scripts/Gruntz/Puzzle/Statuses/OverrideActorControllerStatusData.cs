using Base.Status;
using System;

namespace Gruntz.Puzzle.Statuses
{
    [Serializable]
    public class OverrideActorControllerStatusData : StatusData
    {
        public string AssociatedStatusId;
    }
}
