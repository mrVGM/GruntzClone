using System.Linq;
using UnityEngine;

namespace Base.UI
{
    public class UserInteraction : MonoBehaviour
    {
        public BrainDef BrainDef;
        public GameObject InitialProcessGO;
        public void Init()
        {
            var game = Game.Instance;
            var gameContext = game.Context;
            var brain = gameContext.GetRuntimeObject(BrainDef);
            var brainTag = game.DefRepositoryDef.AllDefs.OfType<BrainProcessContextTagDef>().FirstOrDefault();

            var context = new ProcessContext();
            context.PutItem(brainTag, brain);

            var initialProcess = InitialProcessGO.GetComponent<IProcess>();
            initialProcess.StartProcess(context);
        }
    }
}
