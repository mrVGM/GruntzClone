using Base;
using LevelResults;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelSelection
{
    public class LevelSelectionMap : IContextObject
    {
        public class Edge
        {
            public Site Site1;
            public Site Site2;
            public BezierLine Bridge;

            public IEnumerable<Site> SitesInvolved
            {
                get
                {
                    yield return Site1;
                    yield return Site2;
                }
            }
        }

        public class Neighbour
        {
            public class EdgeDirection
            {
                public BezierLine Bridge;
                public bool Forward = true;
            }
            public EdgeDirection Bridge;
            public Site Site;
        }

        public List<Edge> Edges = new List<Edge>();
        public LevelSelectionMapUnit Unit;

        public void DisposeObject()
        {
        }

        public static LevelSelectionMap GetLevelSelectionMapFromContext()
        {
            var game = Game.Instance;
            var levelSelectionMapDef = game.DefRepositoryDef.AllDefs.OfType<LevelSelectionMapDef>().First();
            return game.Context.GetRuntimeObject(levelSelectionMapDef) as LevelSelectionMap;
        }

        public void InitMap(IEnumerable<Site> sites, IEnumerable<BezierLine> bridges, LevelSelectionMapUnit unit, Site currentSite)
        {
            Unit = unit;
            Unit.CurrentSite = currentSite;
            Unit.transform.position = currentSite.transform.position;

            var levelProgres = LevelProgressInfo.GetLevelProgressInfoFromContext();
            foreach (var bridge in bridges) {
                var leftSite = sites.FirstOrDefault(x => (x.transform.position - bridge.BezierLinePoints.FirstOrDefault().transform.position).sqrMagnitude < 0.1f);
                var rightSite = sites.FirstOrDefault(x => (x.transform.position - bridge.BezierLinePoints.LastOrDefault().transform.position).sqrMagnitude < 0.1f);
                var edge = new Edge { Site1 = leftSite, Site2 = rightSite, Bridge = bridge };
                Edges.Add(edge);

                bridge.Lock.SetActive(true);
                var leftLevelProvider = leftSite as ILevelProvider;
                var rightLevelProvider = rightSite as ILevelProvider;

                if ((leftLevelProvider == null || levelProgres.IsLevelUnlocked(leftLevelProvider.LevelDef)) 
                    && (rightLevelProvider == null || levelProgres.IsLevelUnlocked(rightLevelProvider.LevelDef))) {
                    bridge.Lock.SetActive(false);
                }
            }
        }

        public IEnumerable<Neighbour> GetNeighbours(Site site)
        {
            foreach (var edge in Edges) {
                if (!edge.SitesInvolved.Contains(site)) {
                    continue;
                }
                if (edge.Bridge.Lock.activeSelf) {
                    continue;
                }

                var otherSite = edge.SitesInvolved.FirstOrDefault(x => x != site);
                var bridgeDirection = new Neighbour.EdgeDirection {
                    Bridge = edge.Bridge,
                    Forward = edge.Site2 == otherSite
                };

                yield return new Neighbour {
                    Bridge = bridgeDirection,
                    Site = otherSite
                };
            }
        }

        public IEnumerable<Neighbour> FindPath(Site site1, Site site2) {
            if (site1 == site2) {
                return Enumerable.Empty<Neighbour>();
            }

            Queue<IEnumerable<Neighbour>> paths = new Queue<IEnumerable<Neighbour>>();
            paths.Enqueue(Enumerable.Empty<Neighbour>());

            while (paths.Count > 0) {
                var cur = paths.Dequeue();
                var site = site1;
                var last = cur.LastOrDefault();

                if (last != null) {
                    site = last.Site;
                }

                var neighbours = GetNeighbours(site).Where(x => {
                    if (x.Site == site1) {
                        return false;
                    }

                    if (cur.Select(y => y.Site).Contains(x.Site)) {
                        return false;
                    }
                    return true;
                });

                foreach (var n in neighbours) {
                    var curPath = cur.Concat(new[] { n });
                    if (n.Site == site2) {
                        return curPath;
                    }
                    paths.Enqueue(curPath);
                }
            }
            return Enumerable.Empty<Neighbour>();
        }
    }
}
