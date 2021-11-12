#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LevelSelection
{
    [CustomEditor(typeof(BezierLine))]
    public class BezierLineInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            var bezierLine = target as BezierLine;
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Points")) {
                bezierLine.GeneratePoints();
                EditorSceneManager.MarkAllScenesDirty();
            }
            if (GUILayout.Button("Create Line")) {
                bezierLine.CreateLine();
                EditorSceneManager.MarkAllScenesDirty();
            }
            if (GUILayout.Button("Toggle Handles")) {
                var renderers = bezierLine.GetComponentsInChildren<Renderer>();
                foreach (var rend in renderers) {
                    var bezierPoint = rend.GetComponentInParent<BezierLinePoint>();
                    if (bezierPoint == null) {
                        continue;
                    }
                    rend.enabled = !rend.enabled;
                }
                EditorSceneManager.MarkAllScenesDirty();
            }
        }
    }
}
#endif
