using System.Collections.Generic;
using System.Linq;

namespace Base.Animations
{
    public class AnimationsUpdateOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1).Append(MainUpdaterUpdateTime.Update);
    }
}
