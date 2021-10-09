using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.UnitController
{
    public class UnitControllerExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.Update, 1);
    }
}
