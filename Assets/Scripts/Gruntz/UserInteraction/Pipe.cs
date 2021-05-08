using Base;
using System.Linq;
using UnityEngine;

namespace Gruntz.UserInteraction
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
                return GetChildProcess(childProcessesCount - 1).IsFinished;
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
            var brainDef = game.DefRepositoryDef.AllDefs.OfType<BrainDef>().FirstOrDefault();
            var brain = game.Context.GetRuntimeObject(brainDef) as Brain;
            brain.AddProcess(this);
        }

        public void TerminateProcess()
        {
            var cur = GetChildProcess(curProcess);
            cur.TerminateProcess();
        }
    }
}
