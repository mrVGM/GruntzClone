#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public static class EditorUtils
    {
        public static IEnumerable<T> GetAllAssets<T>() where T : ScriptableObject
        {
            var allDefPaths = AssetDatabase.FindAssets("t:ScriptableObject").Select(x => AssetDatabase.GUIDToAssetPath(x));
            var allDefs = allDefPaths.Select(x => AssetDatabase.LoadAssetAtPath<ScriptableObject>(x));
            var allOfInterest = allDefs.OfType<T>();
            return allOfInterest;
        }

        public static IEnumerable<GameObject> GetAllPrefabs()
        {
            var allPrefabPaths = AssetDatabase.FindAssets("t:Prefab").Select(x => AssetDatabase.GUIDToAssetPath(x));
            var allPrefabs = allPrefabPaths.Select(x => AssetDatabase.LoadAssetAtPath<GameObject>(x));
            return allPrefabs;
        }
    }
}
#endif