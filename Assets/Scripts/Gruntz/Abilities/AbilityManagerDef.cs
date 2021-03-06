using Base;
using Base.MessagesSystem;

namespace Gruntz.Abilities
{
    public class AbilityManagerDef : Def, IRuntimeInstance
    {
        public AbilityManagerOrderTagDef AbilityManagerOrderTagDef;
        public MessagesBoxTagDef AbilityMessages;

        IContextObject IRuntimeInstance.CreateRuntimeInstance()
        {
            return new AbilityManager();
        }
    }
}
