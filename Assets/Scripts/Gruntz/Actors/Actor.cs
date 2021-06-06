using Base;
using Gruntz.Navigation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Actors
{
    public class Actor : ISerializedObject
    {
        ActorData _actorData;
        List<IActorComponent> _components { get; } = new List<IActorComponent>();
        public ActorComponent ActorComponent { get; private set; }
        
        public Vector3 Pos => GetComponent<NavAgent>().Pos;

        public ISerializedObjectData Data
        {
            get
            {
                return _actorData;
            }
            set
            {
                _actorData = value as ActorData;
            }
        }

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

            var addedComponents = _actorData.ActorComponents.Select(x => {
                var comp = x.Component.CreateActorComponent(this);
                _components.Add(comp);
                return new KeyValuePair<IActorComponent, ISerializedObjectData>(comp, x.Data);
            }).ToList();

            foreach (var component in addedComponents)
            {
                component.Key.Init();
                var serializedObject = component.Key as ISerializedObject;
                if (serializedObject != null && component.Value != null)
                {
                    serializedObject.Data = component.Value;
                }
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
