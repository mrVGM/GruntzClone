using Base.Status;
using UnityEngine;

namespace Gruntz.SwitchState
{
    public class GameObjectEnabledStateBehaviour : MonoBehaviour, ISwitchStateBehaviour
    {
        public GameObject GameObject;

        public StatusDef Active;
        public StatusDef Inactive;

        void ISwitchStateBehaviour.SwitchState(StatusDef statusDef)
        {
            if (statusDef == Active) {
                GameObject.SetActive(true);
            }
            if (statusDef == Inactive) {
                GameObject.SetActive(false);
            }
        }
    }
}
