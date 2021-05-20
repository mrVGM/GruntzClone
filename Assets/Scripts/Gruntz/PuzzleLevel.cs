using Base;
using Gruntz.Actors;
using System.Linq;
using UnityEngine;

namespace Gruntz
{
    public class PuzzleLevel : MonoBehaviour
    {
        public Transform ActorContainer;
        public void LevelLoaded()
        {
            var game = Game.Instance;
            var actorManagerDef = game.DefRepositoryDef.AllDefs.OfType<ActorManagerDef>().FirstOrDefault();
            var actorManager = game.Context.GetRuntimeObject(actorManagerDef) as ActorManager;

            var actorComponents = ActorContainer.GetComponentsInChildren<ActorComponent>();
            foreach (var actorComponent in actorComponents)
            {
                var actor = new Actor(actorComponent, actorComponent.ActorData);
                actorManager.AddActor(actor);
            }
        }
    }
}
