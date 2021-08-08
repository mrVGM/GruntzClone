using System;
using UnityEngine;

namespace Gruntz
{
    public class TriggerBoxBehaviour : MonoBehaviour
    {
        public Action<Collider> TriggerEntered;
        public Action<Collider> TriggerExited;
        private void OnTriggerEnter(Collider other)
        {
            TriggerEntered?.Invoke(other);
        }
        private void OnTriggerExit(Collider other)
        {
            TriggerExited?.Invoke(other);
        }
    }
}
