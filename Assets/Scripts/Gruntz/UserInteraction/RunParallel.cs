using Base;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.UserInteraction
{
    public class RunParallel : MonoBehaviour, IProcess
    {
        public int m_Priority;
        IEnumerable<IProcess> childProcesses
        {
            get
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    yield return transform.GetChild(i).GetComponent<IProcess>();
                }
            }
        }

        public bool IsFinished => childProcesses.All(x => x.IsFinished);

        public int Priority => m_Priority;

        public void DoUpdate()
        {
        }

        public void StartProcess(ProcessContext processContext)
        {
            foreach (var process in childProcesses)
            {
                process.StartProcess(processContext);
            }
        }

        public void TerminateProcess()
        {
            foreach (var process in childProcesses)
            {
                process.TerminateProcess();
            }
        }
    }
}
