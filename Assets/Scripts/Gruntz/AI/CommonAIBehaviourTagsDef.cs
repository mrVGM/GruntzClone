using System.Linq;
using Base;
using Base.UI;

namespace Gruntz.AI
{
    public class CommonAIBehaviourTagsDef : Def
    {
        public static CommonAIBehaviourTagsDef BehaviourTagsDef
        {
            get
            {
                var game = Game.Instance;
                var repo = game.DefRepositoryDef;
                return repo.AllDefs.OfType<CommonAIBehaviourTagsDef>().FirstOrDefault();
            }
        }
        
        public ProcessContextTagDef PosessedActor;
        public ProcessContextTagDef AIActor;
    }
}
