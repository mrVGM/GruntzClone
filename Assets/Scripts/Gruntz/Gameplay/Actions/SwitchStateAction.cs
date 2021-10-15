using Base.Actors;
using Base.Status;
using Gruntz.SwitchState;
using System.Collections.Generic;

namespace Gruntz.Gameplay.Actions
{
    public class SwitchStateAction : IGameplayAction
    {
        public Actor Actor { get; set; }

        public void Execute()
        {
            var switchStateComponent = Actor.GetComponent<SwitchStateComponent>();
            IEnumerator<StatusDef> allStatesLooped()
            {
                var def = switchStateComponent.SwitchStateComponentDef;
                int index = 0;
                while (true) {
                    index %= def.StateStatuses.Length;
                    var cur = switchStateComponent.SwitchStateComponentDef.StateStatuses[index++];
                    yield return cur;
                }
            }

            var allStates = allStatesLooped();
            allStates.MoveNext();

            var curState = switchStateComponent.GetCurrentState();
            while (allStates.Current != curState) {
                allStates.MoveNext();
            }

            allStates.MoveNext();
            switchStateComponent.SetCurrentState(allStates.Current);
        }
    }
}
