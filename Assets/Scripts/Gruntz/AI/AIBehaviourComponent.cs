using Base;
using Base.Actors;
using Base.Status;
using Base.UI;
using Gruntz.Statuses;
using System.Linq;
using UnityEngine;

namespace Gruntz.AI
{
    public class AIBehaviourComponent : IActorComponent
    {
        public Actor Actor { get; }
        public AIBehaviourComponentDef AIBehaviourComponentDef;
        
        public AIBehaviourComponent(Actor actor, AIBehaviourComponentDef aiScriptsComponentDef)
        {
            Actor = actor;
            AIBehaviourComponentDef = aiScriptsComponentDef;
        }

        public void DeInit()
        {
        }

        public void Init()
        {
            var game = Game.Instance;
            var gameContext = game.Context;
            var brain = gameContext.GetRuntimeObject(AIBehaviourComponentDef.BrainDef);
            var brainTag = game.DefRepositoryDef.AllDefs.OfType<BrainProcessContextTagDef>().FirstOrDefault();
            var actorIDStatusDef = game.DefRepositoryDef.AllDefs.OfType<ActorIDStatusDef>().FirstOrDefault();

            var context = new ProcessContext();
            context.PutItem(brainTag, brain);
            context.PutItem(AIBehaviourComponentDef.AIActor, Actor);

            var behaviourRoot = Actor.ActorComponent.GetComponent<AIBehaviourRoot>();
            var processToStart = behaviourRoot.ProcessToStart.GetComponent<IProcess>();
            processToStart.StartProcess(context);
        }
    }
}
