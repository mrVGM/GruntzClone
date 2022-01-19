using Base;

namespace Gruntz.AI
{
    public class AIBlackboardDef : Def, IRuntimeInstance
    {
        public AIExecutionOrderTagDef AIExecutionOrderTagDef;
        public IContextObject CreateRuntimeInstance()
        {
            return new AIBlackboard(this);
        }
    }
}
