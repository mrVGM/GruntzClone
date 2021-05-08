using System.Collections.Generic;
using System.Linq;
using Base;

namespace Gruntz.UserInteraction
{
    public class Brain : IOrderedUpdate, IContextObject
    {
        private List<IProcess> activeProcesses = new List<IProcess>();

        public ExecutionOrderTagDef OrderTagDef 
        {
            get
            {
                var game = Game.Instance;
                return game.DefRepositoryDef.AllDefs.OfType<UserInteractionExecutionOrderTagDef>().FirstOrDefault();
            }
        }

        public void DoUpdate()
        {
            var running = activeProcesses.Where(x => !x.IsFinished);
            var sortedProcesses = running.OrderBy(x => x.Priority).ToList();
            activeProcesses.Clear();

            foreach(var process in sortedProcesses)
            {
                process.DoUpdate();
                activeProcesses.Add(process);
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

        public Brain()
        {
            Game.Instance.MainUpdater.RegisterUpdatable(this);
        }
    }
}
