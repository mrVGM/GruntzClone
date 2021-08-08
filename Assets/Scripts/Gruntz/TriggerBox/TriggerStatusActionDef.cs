using Base;
using Gruntz.Actors;
using Gruntz.Status;

namespace Gruntz
{
    public class TriggerStatusActionDef : Def
    {
        public StatusDef StatusDef;
        public virtual Status.Status GetStatus(Actor source, Actor target)
        {
            return StatusDef.Data.CreateStatus();
        }
    }
}
