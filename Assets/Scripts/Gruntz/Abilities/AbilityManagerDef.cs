using Base;
using Base.MessagesSystem;

namespace Gruntz.Abilities
{
    public class AbilityManagerDef : Def, IRuntimeInstance
    {
        public MessagesBoxTagDef AbilityMessages;
        public MessagesBoxTagDef NavigationMessages;

        IContextObject IRuntimeInstance.CreateRuntimeInstance()
        {
            return new AbilityManager();
        }
    }
}
