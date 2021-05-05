namespace Gruntz.UserInteraction
{
    public class Pipe : Process
    {
        int childProcessesCount => transform.childCount;
        Process GetChildProcess(int index)
        {
            return transform.GetChild(index).GetComponent<Process>();
        }

        int curProcess = 0;

        public override bool IsFinished 
        {
            get 
            {
                if (isTermninated)
                {
                    return GetChildProcess(curProcess).IsFinished;
                }
                return GetChildProcess(childProcessesCount - 1).IsFinished;
            }
        }

        public override void DoUpdate()
        {
            if (justScheduled)
            {
                Init();
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

        private void Init()
        {
            curProcess = 0;
            GetChildProcess(curProcess).StartProcess(context);
        }
    }
}
