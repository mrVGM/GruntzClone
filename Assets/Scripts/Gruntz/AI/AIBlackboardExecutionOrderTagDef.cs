using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.AI
{
    public class AIBlackboardExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.Update, 1);
    }
}
