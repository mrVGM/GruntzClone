using System.Collections.Generic;
using Base.UI;
using UnityEngine;

namespace Gruntz.UI
{
    public class ShowObject : CoroutineProcess
    {
        public GameObject ObjectToShow;
        protected override IEnumerator<object> Crt()
        {
            ObjectToShow.SetActive(true);
            while (true) {
                yield return true;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            ObjectToShow.SetActive(false);
            yield break;
        }
    }
}
