using System.Collections.Generic;

namespace Base.Gameplay
{
    public abstract class GameplayActionsProcessorDef : Def
    {
        protected class ProcessResultt
        {
            public List<IGameplayAction> ProcessedActions;
            public bool Dirty;
        }

        protected delegate ProcessResultt ProcessAction(IEnumerable<IGameplayAction> actions);
        protected abstract ProcessAction[] ProccessActions { get; }

        public void ProcessActions(IEnumerable<IGameplayAction> actions)
        {
            var unprocessed = actions;
            int index = 0;
            while (index < ProccessActions.Length) {
                var cur = ProccessActions[index];
                var res = cur(unprocessed);
                unprocessed = res.ProcessedActions;
                ++index;
                if (res.Dirty) {
                    index = 0;
                }
            }
        }

    }
}
