using System.Diagnostics;
using UnityEngine;

namespace Utils
{
    public class DestroyAfterTime : MonoBehaviour
    {
        public float Seconds;

        Stopwatch stopwatch;
        private void Awake()
        {
            stopwatch = Stopwatch.StartNew();
        }
        void Update()
        {
            if (stopwatch.Elapsed.Seconds > Seconds) 
            {
                Destroy(gameObject);
            }
        }
    }
}