using System;
using Base;
using Base.Gameplay;
using Base.MessagesSystem;
using Base.Navigation;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Gruntz.Pushback
{
    [Serializable]
    public class BeingPushedNavAgentControllerDef : Def
    {
        public BeingPushedNavAgentController CreateController(NavAgent navAgent, Vector3 pushDestination, Vector3 pushSnappedOrigin)
        {
            return new BeingPushedNavAgentController(this, navAgent, pushDestination, pushSnappedOrigin);
        }
    }
}
