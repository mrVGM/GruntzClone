using Base;
using Base.MessagesSystem;
using System.Collections.Generic;
using System.Linq;
using static Base.Animations.AnimationsComponent;

namespace Gruntz.Abilities
{
    public class AbilityManager : IContextObject, IOrderedUpdate
    {
        public ExecutionOrderTagDef OrderTagDef => AbilityManagerDef.AbilityManagerOrderTagDef;

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
            AbilityPlayers = AbilityPlayers.Where(x => x.State.GeneralState == AbilityPlayer.GeneralExecutionState.Playing).ToList();
            var cache = AbilityPlayers.ToList();
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();


            foreach (var ability in cache) {
                ability.Update();

                if (ability.State.AnimationState == AbilityPlayer.AnimationExecutionState.AnimationPlaying) {
                    messagesSystem.SendMessage(AbilityManagerDef.AbilityMessages,
                        MainUpdaterUpdateTime.FixedCrt,
                        this,
                        new AbilityAnimationInfo {
                            Actor = ability.Actor,
                            Animation = ability.AbilityDef.Animation,
                        });
                }
            }
        }
    }
}
