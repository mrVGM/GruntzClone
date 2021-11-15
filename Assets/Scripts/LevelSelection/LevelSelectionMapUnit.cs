using Base.MessagesSystem;
using System.Collections.Generic;
using UnityEngine;
using static LevelSelection.LevelSelectionMap;
using static LevelSelection.LevelSelectionMap.Neighbour;

namespace LevelSelection
{
    public class LevelSelectionMapUnit : MonoBehaviour
    {
        public struct WalkInfo
        {
            public bool Walking;
            public Transform SegmentStart;
            public Transform SegmentEnd;
        }

        public float Speed = 1;

        private Site _targetSite;
        private Neighbour _currentCrossing = null;
        private Queue<Neighbour> _currentPath = new Queue<Neighbour>();
        IEnumerator<WalkInfo> _walk = null;
        private WalkInfo _curWalkInfo;

        public bool IsWalking => _curWalkInfo.Walking;
        public Site CurrentSite { get; private set; }

        public Site TargetSite
        {
            set
            {
                if (_targetSite == value) {
                    return;
                }
                _targetSite = value;

                var startingSite = CurrentSite;
                if (_currentCrossing != null) {
                    startingSite = _currentCrossing.Site;
                }
                _currentPath.Clear();
                if (startingSite == _targetSite) {
                    return;
                }

                var map = GetLevelSelectionMapFromContext();
                var path = map.FindPath(startingSite, _targetSite);
                foreach (var step in path) {
                    _currentPath.Enqueue(step);
                }
            }
        }

        IEnumerator<WalkInfo> GenerateWalkInfo()
        {
            IEnumerable<WalkInfo> walkAlongEdge(EdgeDirection edge)
            {
                if (edge.Forward) {
                    for (int i = 0; i < edge.Bridge.GeneratedPoints.childCount - 1; ++i) {
                        yield return new WalkInfo {
                            Walking = true,
                            SegmentStart = edge.Bridge.GeneratedPoints.GetChild(i),
                            SegmentEnd = edge.Bridge.GeneratedPoints.GetChild(i + 1)
                        };
                    }
                }
                else {
                    for (int i = edge.Bridge.GeneratedPoints.childCount - 1; i > 0; --i) {
                        yield return new WalkInfo {
                            Walking = true,
                            SegmentStart = edge.Bridge.GeneratedPoints.GetChild(i),
                            SegmentEnd = edge.Bridge.GeneratedPoints.GetChild(i - 1)
                        };
                    }
                }
            }

            while (true) {
                if (_currentCrossing == null && _currentPath.Count > 0) {
                    _currentCrossing = _currentPath.Dequeue();
                }

                if (_currentCrossing == null) {
                    yield return new WalkInfo { Walking = false };
                    continue;
                }

                var walkAlong = walkAlongEdge(_currentCrossing.Bridge);

                foreach (var step in walkAlong) {
                    yield return step;
                }
                CurrentSite = _currentCrossing.Site;
                _currentCrossing = null;
            }
        }

        private void UpdatePos()
        {
            if (_walk == null) {
                _walk = GenerateWalkInfo();
                _walk.MoveNext();
                _curWalkInfo = _walk.Current;
            }

            if (!_curWalkInfo.Walking) {
                _walk.MoveNext();
                _curWalkInfo = _walk.Current;
            }

            float distToCover = Time.fixedDeltaTime * Speed;
            while (distToCover > 0) {
                if (!_curWalkInfo.Walking) {
                    break;
                }
                Vector3 offset = _curWalkInfo.SegmentEnd.transform.position - transform.position;
                float distToSegmentEnd = offset.magnitude;
                if (distToSegmentEnd <= 0) {
                    _walk.MoveNext();
                    _curWalkInfo = _walk.Current;
                    continue;
                }

                if (distToSegmentEnd <= distToCover) {
                    transform.position = _curWalkInfo.SegmentEnd.position;
                    transform.rotation = Quaternion.LookRotation( offset);
                    distToCover -= distToSegmentEnd;
                    continue;
                }

                transform.position += distToCover * offset.normalized;
                transform.rotation = Quaternion.LookRotation(offset);
                distToCover = 0;
            }
        }

        private void FixedUpdate()
        {
            UpdatePos();
            var animator = GetComponentInChildren<Animator>();
            if (_curWalkInfo.Walking) {
                animator.SetInteger("State", 1);
            }
            else {
                animator.SetInteger("State", 0);
            }
        }

        public void TeleportTo(Site site)
        {
            transform.position = site.transform.position;
            CurrentSite = site;
            TargetSite = site;
            _currentCrossing = null;
            _currentPath.Clear();
            _currentCrossing = null;
        }
    }
}
