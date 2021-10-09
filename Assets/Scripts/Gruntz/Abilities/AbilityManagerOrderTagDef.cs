using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Abilities
{
    public class AbilityManagerOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1);
    }
}
