using System.Collections.Generic;
using System.Linq;

namespace Base.MessagesSystem
{
    public class MessagesSystemExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1).Append(MainUpdaterUpdateTime.Update);
    }
}
