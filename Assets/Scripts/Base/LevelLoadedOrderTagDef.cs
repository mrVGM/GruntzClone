using System.Collections.Generic;
using System.Linq;

namespace Base
{
    public class LevelLoadedOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedUpdate, 1);
    }
}
