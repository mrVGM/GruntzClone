using System.Collections.Generic;
using Base.UI;
using UnityEngine;

namespace Gruntz.UI
{
    public class WaitForInput : CoroutineProcess
    {
        public string InputAxis;
        protected override IEnumerator<object> Crt()
        {
            while (Input.GetAxis(InputAxis) <= 0) {
                yield return null;
            }

            while (Input.GetAxis(InputAxis) > 0) {
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
