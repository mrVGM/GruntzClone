using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.Puzzle.Gameplay
{
    public class TrueFilterNode : MonoBehaviour, IFilterNode
    {
        public bool Filter(IEnumerable<GameplayEvent> gameplayEvents)
        {
            return true;
        }
    }
}
