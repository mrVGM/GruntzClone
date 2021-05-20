using System;
using Base;

namespace Gruntz.Navigation
{
    public class NavAgentDef : Def
    {
        [Serializable]
        public class NavAgentStats 
        {
            public float Speed;
        }

        public NavAgentStats NavStats;
    }
}
