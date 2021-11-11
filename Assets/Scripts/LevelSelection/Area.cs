using System.Collections.Generic;
using UnityEngine;

namespace LevelSelection
{
    public class Area : MonoBehaviour
    {
        public string Label;
        public Site InitialSite;
        public Transform SitesContainer;
        public Transform BridgesContainer;

        public IEnumerable<BezierLine> Bridges => BridgesContainer.GetComponentsInChildren<BezierLine>();
        public IEnumerable<Site> Sites => SitesContainer.GetComponentsInChildren<Site>();
    }
}
