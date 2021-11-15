using Base;
using Base.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gruntz.SwitchState
{
    public class MultiStateBehaviour : MonoBehaviour, ISwitchStateBehaviour
    {
        public GameObject[] ChildBehaviours;
        public IEnumerable<ISwitchStateBehaviour> SwitchStateBehaviours => ChildBehaviours
            .Select(x => x.GetComponent<ISwitchStateBehaviour>()).Where(x => x != null);

        void ISwitchStateBehaviour.SwitchState(StatusDef statusDef)
        {
            foreach (var beh in SwitchStateBehaviours) {
                beh.SwitchState(statusDef);
            }
        }
    }
}
