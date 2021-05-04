using System.Linq;
using UnityEditor;

namespace Base
{
    public class SaveAssetsProcessor : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            var allDefPaths = AssetDatabase.FindAssets("t:Def").Select(x => AssetDatabase.GUIDToAssetPath(x));
            var allDefs = allDefPaths.Select(x => AssetDatabase.LoadAssetAtPath<Def>(x));
            var defRepo = allDefs.OfType<DefRepositoryDef>().FirstOrDefault();
            defRepo.AllDefs = allDefs.ToArray();
            EditorUtility.SetDirty(defRepo);
            return paths;
        }
    }
}
