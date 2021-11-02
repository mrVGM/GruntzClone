#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Base
{
    [InitializeOnLoad]
    public static class SaveSceneProcessor
    {
        static SaveSceneProcessor()
        {
            EditorSceneManager.sceneSaving += SceneSaving;
        }

        public static void SceneSaving(Scene scene, string path)
        {
            var sceneIDs = scene.GetRootGameObjects().Select(x => x.GetComponent<SceneIDs>()).FirstOrDefault(x => x != null); 
            if (sceneIDs != null)
            {
                sceneIDs.Refresh(scene);
            }
        }
    }
}
#endif
