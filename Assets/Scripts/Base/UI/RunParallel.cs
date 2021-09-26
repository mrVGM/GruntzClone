using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.UI
{
    public class RunParallel : MonoBehaviour, IProcess
    {
        public int m_Priority;
        public ProcessIDTagDef[] WatchForTermination;
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
            string name = gameObject.name;
            var finishedProcesses = childProcesses.Where(x => x.IsFinished);
            var finishedProcessesIDs = finishedProcesses.Select(x => x.ID).Where(x => x != null);
            if (WatchForTermination.Intersect(finishedProcessesIDs).Any())
            {
                TerminateProcess();
            }
        }

        public void StartProcess(ProcessContext processContext)
        {
            foreach (var process in childProcesses)
            {
                process.StartProcess(processContext);
            }

            var game = Game.Instance;
            var brainDef = game.DefRepositoryDef.AllDefs.OfType<BrainDef>().FirstOrDefault();
            var brain = game.Context.GetRuntimeObject(brainDef) as Brain;
            brain.AddProcess(this);
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
