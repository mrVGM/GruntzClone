namespace Gruntz.AI
{
    public interface IAIAction
    {
        bool CanProceed();
        void Update();
        void Stop();
    }
}
