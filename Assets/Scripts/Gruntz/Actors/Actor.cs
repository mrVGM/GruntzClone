using Base;
using Gruntz.Navigation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Actors
{
    public class Actor
    {
        ActorData _actorData;
        List<IActorComponent> _components { get; } = new List<IActorComponent>();
        public ActorComponent ActorComponent { get; private set; }
        
        public Vector3 Pos => GetComponent<NavAgent>().Pos;

        public Actor(ActorComponent actorComponent, ActorData actorData)
        {
            _actorData = actorData;
            ActorComponent = actorComponent;
            ActorComponent.ActorData = _actorData;
        }
        public void Init()
        {
            var game = Game.Instance;
            var actorManagerDef = game.DefRepositoryDef.AllDefs.OfType<ActorManagerDef>().FirstOrDefault();
            var actorManager = game.Context.GetRuntimeObject(actorManagerDef) as ActorManager;
            actorManager.AddActor(this);

            foreach (var componentDef in _actorData.ActorComponents)
            {
                _components.Add(componentDef.CreateActorComponent(this));
            }

            foreach (var component in _components)
            {
                component.Init();
            }
        }

        public void Deinit()
        {
            foreach (var component in _components)
            {
                component.DeInit();
            }
        }

        public T GetComponent<T>() where T : IActorComponent
        {
            return _components.OfType<T>().FirstOrDefault();
        }
    }
}
