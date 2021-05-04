namespace Base
{
    public interface IOrderedUpdate
    {
        ExecutionOrderTagDef OrderTagDef { get; }
        void DoUpdate();
    }
}
