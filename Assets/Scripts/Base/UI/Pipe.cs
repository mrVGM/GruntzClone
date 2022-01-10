using System.Linq;
using UnityEngine;

namespace Base.UI
{
    public class Pipe : MonoBehaviour, IProcess
    {
        public int m_Priority;
        private ProcessContext context;
        int childProcessesCount => transform.childCount;
        IProcess GetChildProcess(int index)
        {
            return transform.GetChild(index).GetComponent<IProcess>();
        }

        int curProcess = 0;
        bool terminated = false;

        public bool IsFinished
        {
            get 
            {
                if (terminated)
                {
                    return GetChildProcess(curProcess).IsFinished;
                }
                if (curProcess != childProcessesCount - 1) {
                    return false;
                }
                return GetChildProcess(curProcess).IsFinished;
            }
        }

        public int Priority => m_Priority;

        public ProcessIDTagDef ID
        {
            get
            {
                var taggedObject = GetComponent<TaggedObject>();
                if (taggedObject == null)
                {
                    return null;
                }
                return taggedObject.Tag as ProcessIDTagDef;
            }
        }

        public void DoUpdate()
        {
            if (terminated) {
                return;
            }

            if (GetChildProcess(curProcess).IsFinished)
            {
                if (curProcess < childProcessesCount - 1)
                {
                    ++curProcess;
                    GetChildProcess(curProcess).StartProcess(context);
                }
            }
        }

        public void StartProcess(ProcessContext processContext)
        {
            curProcess = 0;
            terminated = false;
            context = processContext;
            var initialProcess = GetChildProcess(curProcess);
            initialProcess.StartProcess(context);

            var game = Game.Instance;
            var brainProcessTagDef = game.DefRepositoryDef.AllDefs.OfType<BrainProcessContextTagDef>().FirstOrDefault();
            var brain = processContext.GetItem(brainProcessTagDef) as Brain;
            brain.AddProcess(this);
        }

        public void TerminateProcess()
        {
            terminated = true;
            var cur = GetChildProcess(curProcess);
            cur.TerminateProcess();
        }
    }
}
