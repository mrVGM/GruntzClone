using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Gameplay
{
    public class GameplayManagerExecutionOrderTag : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1);
    }
}
