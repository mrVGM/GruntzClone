using System;
using Base;
using Base.Navigation;
using Gruntz.Statuses;
using UnityEngine;

namespace Gruntz.Pushback
{
    [Serializable]
    public class BeingPushedNavAgentControllerDef : Def
    {
        public PushStatusDef PushStatusDef;
        public BeingPushedNavAgentController CreateController(NavAgent navAgent, Vector3 pushDestination, Vector3 pushSnappedOrigin)
        {
            return new BeingPushedNavAgentController(this, navAgent, pushDestination, pushSnappedOrigin);
        }
    }
}
