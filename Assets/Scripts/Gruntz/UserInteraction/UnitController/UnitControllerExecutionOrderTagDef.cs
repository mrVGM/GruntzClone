using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.UserInteraction.UnitController
{
    public class UnitControllerExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1).Append(MainUpdaterUpdateTime.Update);
    }
}
