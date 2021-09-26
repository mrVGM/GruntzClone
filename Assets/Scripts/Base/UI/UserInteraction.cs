using UnityEngine;

namespace Base.UI
{
    public class UserInteraction : MonoBehaviour
    {
        public GameObject InitialProcessGO;
        public void Init()
        {
            var context = new ProcessContext();
            var initialProcess = InitialProcessGO.GetComponent<IProcess>();
            initialProcess.StartProcess(context);
        }
    }
}
