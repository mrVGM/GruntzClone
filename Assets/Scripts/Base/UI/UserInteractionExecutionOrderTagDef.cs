using System.Collections.Generic;
using System.Linq;

namespace Base.UI
{
    public class UserInteractionExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.Update, 1);
    }
}
