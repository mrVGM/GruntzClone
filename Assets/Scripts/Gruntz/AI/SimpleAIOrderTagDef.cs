using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.AI
{
    public class SimpleAIOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.Update, 1);
    }
}
