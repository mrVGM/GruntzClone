using System.Collections.Generic;
using Base.UI;
using UnityEngine;

namespace Gruntz.UI
{
    public class EnableGameObject : CoroutineProcess
    {
        public GameObject GO;
        public bool Enabled = true;
        
        protected override IEnumerator<object> Crt()
        {
            GO.SetActive(Enabled);
            yield break;
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
