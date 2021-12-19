using System.Collections.Generic;
using System.Linq;

namespace Base.UI
{
    public class Brain : IOrderedUpdate, IContextObject
    {
        public BrainDef BrainDef { get; }
        private List<IProcess> activeProcesses = new List<IProcess>();

        public ExecutionOrderTagDef OrderTagDef => BrainDef.ExecutionOrderTagDef;

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            var running = activeProcesses.Where(x => !x.IsFinished);
            var sortedProcesses = running.OrderBy(x => x.Priority).ToList();
            activeProcesses.Clear();

            foreach(var process in sortedProcesses)
            {
                process.DoUpdate();
                if (!process.IsFinished)
                {
                    activeProcesses.Add(process);
                }
            }
        }

        public void AddProcess(IProcess process)
        {
            activeProcesses.Add(process);
        }

        public void DisposeObject()
        {
            Game.Instance.MainUpdater.UnRegisterUpdatable(this);
        }

        public Brain(BrainDef brainDef)
        {
            BrainDef = brainDef;
            Game.Instance.MainUpdater.RegisterUpdatable(this);
        }
    }
}
