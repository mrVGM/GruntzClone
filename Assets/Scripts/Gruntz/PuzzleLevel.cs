using Gruntz.Navigation;
using UnityEngine;

namespace Gruntz
{
    public class PuzzleLevel : MonoBehaviour
    {
        public Transform AgentsContainer;
        public void LevelLoaded()
        {
            var agents = AgentsContainer.GetComponentsInChildren<NavAgent>();
            foreach (var agent in agents)
            {
                agent.Init();
            }
        }
    }
}
