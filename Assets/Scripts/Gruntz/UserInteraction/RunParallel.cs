using System.Collections.Generic;
using System.Linq;

namespace Gruntz.UserInteraction
{
    public class RunParallel : Process
    {
        public Process[] TerminateOn;

        IEnumerable<Process> childProcesses
        {
            get
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    yield return transform.GetChild(i).GetComponent<Process>();
                }
            }
        }

        public override bool IsFinished => childProcesses.All(x => x.IsFinished);

        public override void DoUpdate()
        {
            if (justScheduled)
            {
                foreach (var process in childProcesses)
                {
                    process.StartProcess(context);
                }
            }
            if (TerminateOn.Any(x => x.IsFinished))
            {
                TerminateProcess();
            }
            if (isTermninated)
            {
                foreach (var process in childProcesses)
                {
                    process.TerminateProcess();
                }
            }
        }
    }
}
