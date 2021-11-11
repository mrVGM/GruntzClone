using Base;
using System.Linq;

namespace LevelResults
{
    public class LevelResultHolder : IContextObject
    {
        public LevelDef Level;
        public object LevelResult;
        public void DisposeObject()
        {
        }
        public static LevelResultHolder GetLevelResultHolderFromContext()
        {
            var game = Game.Instance;
            var levelResultHolderDef = game.DefRepositoryDef.AllDefs.OfType<LevelResultHolderDef>().First();
            return game.Context.GetRuntimeObject(levelResultHolderDef) as LevelResultHolder;
        }
    }
}
