using UnityEditor;
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
            }
            if (GUILayout.Button("Create Line")) {
                bezierLine.CreateLine();
            }
            if (GUILayout.Button("Toggle Handles")) {
                var renderers = bezierLine.GetComponentsInChildren<Renderer>();
                foreach (var rend in renderers) {
                    rend.enabled = !rend.enabled;
                }
            }
        }
    }
}
