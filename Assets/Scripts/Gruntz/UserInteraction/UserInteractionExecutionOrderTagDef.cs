using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.UserInteraction
{
    public class UserInteractionExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.Update, 1);
    }
}
