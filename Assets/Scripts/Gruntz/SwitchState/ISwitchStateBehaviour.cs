using Base.Status;

namespace Gruntz.SwitchState
{
    public interface ISwitchStateBehaviour
    {
        void SwitchState(StatusDef statusDef);
    }
}
