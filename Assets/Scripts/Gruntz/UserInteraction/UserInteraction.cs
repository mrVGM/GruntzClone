using UnityEngine;

namespace Gruntz.UserInteraction
{
    public class UserInteraction : MonoBehaviour
    {
        public Process InitialProcess;
        public void Init()
        {
            var context = new ProcessContext();
            InitialProcess.StartProcess(context);
        }
    }
}
