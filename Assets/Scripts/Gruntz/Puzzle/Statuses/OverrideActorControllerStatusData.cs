using System;

namespace Gruntz.Puzzle.Statuses
{
    [Serializable]
    public class OverrideActorControllerStatusData : Status.StatusData
    {
        public string AssociatedStatusId;
    }
}
