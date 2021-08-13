using Base;
using UnityEngine;

namespace Gruntz.Puzzle
{
    public class GameplayManagerDef : Def, IRuntimeInstance
    {
        public Transform DecisionTree;
        public IContextObject CreateRuntimeInstance()
        {
            return new GameplayManager(this);
        }
    }
}
