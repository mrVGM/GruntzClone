using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.AI.Processes
{
    public class DoingStuff : CoroutineProcess
    {
        public float LogInterval = 2;
        protected override IEnumerator<object> Crt()
        {
            float lastLog = 0;
            while (true)
            {
                while (Time.time - lastLog < LogInterval) {
                    yield return null;
                }

                Debug.Log("Doing Stuff");
                lastLog = Time.time;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
