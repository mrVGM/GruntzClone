using System.Linq;
using Base;
using UnityEngine;

namespace Gruntz.UserInteraction
{
    public abstract class Process : MonoBehaviour
    {
        public int Priority;
        protected ProcessContext context { get; private set; }
        protected bool isTermninated { get; private set; } = false;
        private bool _justScheduled = false;
        protected bool justScheduled 
        {
            get
            {
                bool res = _justScheduled;
                _justScheduled = false;
                return res;
            }
        }
        public void StartProcess(ProcessContext processContext)
        {
            var defRepo = Game.Instance.DefRepositoryDef;
            var brainDef = defRepo.AllDefs.OfType<BrainDef>().FirstOrDefault();
            var brain = Game.Instance.Context.GetRuntimeObject(brainDef) as Brain;
            context = processContext;
            isTermninated = false;
            _justScheduled = true;
            brain.AddProcess(this);
        }
        public void TerminateProcess()
        {
            isTermninated = true;
        }
        public abstract void DoUpdate();
        public abstract bool IsFinished { get; }
    }
}
