namespace Gruntz.UserInteraction
{
    public class Loop : Process
    {
        private Process childProcess => transform.GetChild(0).GetComponent<Process>();

        public override bool IsFinished 
        {
            get 
            {
                if (!isTermninated) { return false; }
                return childProcess.IsFinished;
            }
        }

        public override void DoUpdate()
        {
            if (justScheduled)
            {
                Init();
                return;
            }
            if (!isTermninated && childProcess.IsFinished) 
            {
                childProcess.StartProcess(context);
            }
        }

        private void Init()
        {
            childProcess.StartProcess(context);
        }
    }
}
