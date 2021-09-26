using System.Collections.Generic;
using System.Linq;

namespace Base.Navigation
{
    public class NavigationRequestOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1);
    }
}
