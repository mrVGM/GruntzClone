using System.Collections.Generic;
using System.Linq;

namespace Base.Navigation
{
    public class NavigationExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1);
    }
}
