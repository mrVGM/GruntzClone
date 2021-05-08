namespace Gruntz.UserInteraction
{
    public interface IProcess
    {
        ProcessIDTagDef ID { get; }
        int Priority { get;  }
        void StartProcess(ProcessContext processContext);
        void TerminateProcess();
        void DoUpdate();
        bool IsFinished { get; }
    }
}
