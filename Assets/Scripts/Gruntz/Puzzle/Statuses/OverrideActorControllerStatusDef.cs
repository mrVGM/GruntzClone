using Base.MessagesSystem;
using Gruntz.Status;

namespace Gruntz.Puzzle.Statuses
{
    public class OverrideActorControllerStatusDef : StatusDef
    {
        public MessagesBoxTagDef MessagesBoxTagDef;
        protected override StatusData StatusData
        {
            get
            {
                return new OverrideActorControllerStatusData();
            }
        }
    }
}
