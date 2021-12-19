using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.UI
{
    public abstract class CoroutineProcess : MonoBehaviour, IProcess
    {
        public int m_Priority;
        private bool terminated = false;
        private bool finished = false;

        protected ProcessContext context { get; private set; }
        private IEnumerator<object> crt;
        void IProcess.StartProcess(ProcessContext processContext)
        {
            finished = false;
            terminated = false;
            context = processContext;
            crt = CombinedCrt();
            
            var game = Game.Instance;
            var brainProcessTagDef = game.DefRepositoryDef.AllDefs.OfType<BrainProcessContextTagDef>().FirstOrDefault();
            var brain = processContext.GetItem(brainProcessTagDef) as Brain;
            brain.AddProcess(this);
        }

        void IProcess.TerminateProcess()
        {
            terminated = true;
        }

        void IProcess.DoUpdate()
        {
            crt.MoveNext();
        }

        int IProcess.Priority => m_Priority;

        bool IProcess.IsFinished => finished;

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

        protected abstract IEnumerator<object> Crt();
        protected abstract IEnumerator<object> FinishCrt();

        private IEnumerator<object> CombinedCrt()
        {
            var crt = Crt();

            while (!terminated && crt.MoveNext())
            {
                yield return null;
            }

            var finishCrt = FinishCrt();
            while (finishCrt.MoveNext())
            {
                yield return null;
            }
            finished = true;
        }
    }
}
