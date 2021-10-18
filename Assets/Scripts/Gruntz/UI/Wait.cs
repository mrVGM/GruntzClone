using System.Collections.Generic;
using Base.UI;

namespace Gruntz.UI
{
    public class Wait : CoroutineProcess
    {
        public float Time = 4.0f;
        protected override IEnumerator<object> Crt()
        {
            float time = UnityEngine.Time.time;
            while (UnityEngine.Time.time - time < Time) {
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
