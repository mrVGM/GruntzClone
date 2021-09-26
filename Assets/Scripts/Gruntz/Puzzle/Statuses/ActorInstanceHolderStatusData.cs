using Base.Actors;
using Base.Status;
using System;

namespace Gruntz.Puzzle.Statuses
{
    [Serializable]
    public class ActorInstanceHolderStatusData : StatusData
    {
        public ActorData ActorData;
    }
}
