using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelSelection
{
    public class BezierLine : MonoBehaviour
    {
        public float LeftLineOffset = 0;
        public float RightLineOffset = 0;

        public float DesiredDist = 0.1f;
        public float Range = 0.01f;
        public Transform GeneratedPoints;
        public BezierLinePoint[] BezierLinePoints => GetComponentsInChildren<BezierLinePoint>();

        public LineRenderer LineRenderer => GetComponentInChildren<LineRenderer>();

        public Vector3 GetMiddlePoint(BezierLinePoint left, BezierLinePoint right, float progress)
        {
            List<Vector3> points = new List<Vector3>();
            points.Add(left.transform.position);
            points.Add(left.RightHandle.position);
            points.Add(right.LeftHandle.position);
            points.Add(right.transform.position);

            IEnumerable<Vector3> process()
            {
                for (int i = 0; i < points.Count - 1; ++i) {
                    yield return (1 - progress) * points[i] + progress * points[i + 1];
                }
            }

            while (points.Count > 1) {
                points = process().ToList();
            }

            return points[0];
        }

        public void GeneratePoints()
        {
            if (GeneratedPoints != null) {
                DestroyImmediate(GeneratedPoints.gameObject);
            }

            GeneratedPoints = new GameObject("GeneratedPoints").transform;
            GeneratedPoints.SetParent(transform);

            float startTime = Time.time;
            IEnumerable<Vector3> positions(BezierLinePoint left, BezierLinePoint right)
            {
                float cur = 0;
                Vector3 curPoint = GetMiddlePoint(left, right, cur);
                Vector3 lastPoint = GetMiddlePoint(left, right, 1);
                yield return curPoint;

                bool checkNextPoint(Vector3 tested) {
                    float offset = (curPoint - tested).magnitude;
                    if (Mathf.Abs(offset - DesiredDist) <= Range) {
                        return true;
                    }
                    return false;
                }

                while (true) {
                    float offset = (curPoint - lastPoint).magnitude;
                    if ((curPoint - lastPoint).magnitude <= DesiredDist) {
                        break;
                    }

                    float l = cur;
                    float r = 1;

                    while (true) {
                        var mid = 0.5f * (l + r);
                        var midPoint = GetMiddlePoint(left, right, mid);
                        if (checkNextPoint(midPoint)) {
                            cur = mid;
                            curPoint = midPoint;
                            yield return curPoint;
                            break;
                        }

                        var d = (curPoint - midPoint).magnitude;
                        if (d < DesiredDist) {
                            l = mid;
                            continue;
                        }
                        if (d > DesiredDist) {
                            r = mid;
                            continue;
                        }
                    }
                }
                yield return lastPoint;
            }

            int index = 0;
            for (int i = 0; i < BezierLinePoints.Length - 1; ++i) {
                foreach (Vector3 pos in positions(BezierLinePoints[i], BezierLinePoints[i + 1])) {
                    var go = new GameObject($"Path Point {index++}");
                    go.transform.SetParent(GeneratedPoints);
                    go.transform.position = pos;
                }
            }
        }

        public void CreateLine()
        {
            int startPoint = Mathf.FloorToInt(LeftLineOffset * GeneratedPoints.childCount);
            int endPoint = Mathf.CeilToInt((1 - RightLineOffset) * GeneratedPoints.childCount);

            IEnumerable<Transform> allPoints()
            {
                for (int i = 0; i < GeneratedPoints.childCount; ++i) {
                    if (i >= startPoint && i < endPoint) {
                        yield return GeneratedPoints.GetChild(i);
                    }
                }
            }

            var pts = allPoints().ToList();
            var lineRenderer = LineRenderer;
            lineRenderer.positionCount = pts.Count;

            for (int i = 0; i < pts.Count; ++i) {
                var cur = pts[i];
                lineRenderer.SetPosition(i, cur.transform.position);
            }
        }
    }
}
