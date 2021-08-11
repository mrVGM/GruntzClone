using System.Collections.Generic;
using System.Linq;

namespace Base
{
    public interface IUpdateTime
    {
        IEnumerable<MainUpdaterUpdateTime> UpdateTime { get; }
    }
    public class ExecutionOrderTagDef : TagDef, IUpdateTime
    {
        public virtual IEnumerable<MainUpdaterUpdateTime> UpdateTime => Enumerable.Repeat(MainUpdaterUpdateTime.Update, 1);
    }
}
