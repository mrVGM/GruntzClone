namespace Base.Navigation
{
    public interface INavAgentController
    {
        NavAgent NavAgent { set; }
        MoveRequest MoveRequest { get; }
    }
}
