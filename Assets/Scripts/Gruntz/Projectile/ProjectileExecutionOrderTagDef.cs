using Base;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Projectile
{
    public class ProjectileExecutionOrderTagDef : ExecutionOrderTagDef
    {
        public override IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.FixedCrt, 1);
    }
}
