using System.Linq;
using UnityEngine;

namespace Base.UI
{
    public class Loop : MonoBehaviour, IProcess
    {
        public int m_Priority;
        private ProcessContext context;
        private bool terminated = false;

        private IProcess childProcess => transform.GetChild(0).GetComponent<IProcess>();

        public bool IsFinished
        {
            get 
            {
                if (!terminated)
                {
                    return false;
                }
                return childProcess.IsFinished;
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
            if (terminated)
            {
                childProcess.TerminateProcess();
                return;
            }
            if (childProcess.IsFinished) 
            {
                childProcess.StartProcess(context);
            }
        }

        public void StartProcess(ProcessContext processContext)
        {
            terminated = false;
            context = processContext;
            childProcess.StartProcess(context);

            var game = Game.Instance;
            var brainDef = game.DefRepositoryDef.AllDefs.OfType<BrainDef>().FirstOrDefault();
            var brain = game.Context.GetRuntimeObject(brainDef) as Brain;
            brain.AddProcess(this);
        }

        public void TerminateProcess()
        {
            terminated = true;
        }
    }
}
