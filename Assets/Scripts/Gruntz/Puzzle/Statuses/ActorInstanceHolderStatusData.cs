using Base.Actors;
using System;

namespace Gruntz.Puzzle.Statuses
{
    [Serializable]
    public class ActorInstanceHolderStatusData : Status.StatusData
    {
        public ActorData ActorData;
    }
}
