using Base.UI;
using UnityEngine;

namespace LevelSelection
{
    public class LevelSelectionUI : MonoBehaviour
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
