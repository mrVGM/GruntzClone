using Base;
using Base.Actors;
using Base.MessagesSystem;
using System.Collections.Generic;
using System.Linq;

namespace Gruntz.Abilities
{
    public class AbilityManager : IContextObject, IOrderedUpdate
    {
        public struct AbilityExecutionInfo
        {
            public Actor Actor;
            public AbilityDef AbilityDef;
            public AbilityPlayer.ExecutionState ExecutionState;
        }
        public ExecutionOrderTagDef OrderTagDef => Game.Instance.DefRepositoryDef.AllDefs.OfType<AbilityManagerOrderTagDef>().FirstOrDefault();

        public List<AbilityPlayer> AbilityPlayers = new List<AbilityPlayer>();

        public AbilityManagerDef AbilityManagerDef { get; private set; }
        public AbilityManager()
        {
            var mainUpdater = Game.Instance.MainUpdater;
            mainUpdater.RegisterUpdatable(this);
        }

        public static AbilityManager GetAbilityManagerFromContext()
        {
            var game = Game.Instance;
            var abilityManagerDef = game.DefRepositoryDef.AllDefs.OfType<AbilityManagerDef>().First();
            var context = game.Context;
            var abilityManager = context.GetRuntimeObject(abilityManagerDef) as AbilityManager;
            abilityManager.AbilityManagerDef = abilityManagerDef;
            return abilityManager;
        }

        public void DisposeObject()
        {
            var mainUpdater = Game.Instance.MainUpdater;
            mainUpdater.UnRegisterUpdatable(this);
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            AbilityPlayers = AbilityPlayers.Where(x => x.State == AbilityPlayer.ExecutionState.Playing).ToList();
            var cache = AbilityPlayers.ToList();
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();

            foreach (var ability in cache) {
                ability.Update();

                messagesSystem.SendMessage(AbilityManagerDef.AbilityMessages,
                    MainUpdaterUpdateTime.FixedCrt,
                    this,
                    new AbilityExecutionInfo {
                    Actor = ability.Actor,
                    AbilityDef = ability.AbilityDef,
                    ExecutionState = ability.State,
                });
            }
        }
    }
}
