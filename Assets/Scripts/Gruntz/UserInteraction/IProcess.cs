namespace Gruntz.UserInteraction
{
    public interface IProcess
    {
        int Priority { get;  }
        void StartProcess(ProcessContext processContext);
        void TerminateProcess();
        void DoUpdate();
        bool IsFinished { get; }
    }
}
