using System.Linq;

namespace Base
{
    public class SceneIDsHolder : IContextObject
    {
        public SceneIDs SceneIDs;
        public void DisposeObject()
        {
        }

        public static SceneIDsHolder GetSceneIDsHolderFromContext()
        {
            var game = Game.Instance;
            var defRepo = game.DefRepositoryDef;
            var sceneIDsDef = defRepo.AllDefs.OfType<SceneIDsDef>().FirstOrDefault();
            var sceneIDsHolder = game.Context.GetRuntimeObject(sceneIDsDef) as SceneIDsHolder;
            return sceneIDsHolder;
        }
    }
}
