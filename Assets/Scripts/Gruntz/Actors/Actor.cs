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
        Dictionary<ActorComponentDef, IActorComponent> _components { get; } = new Dictionary<ActorComponentDef, IActorComponent>();
        public ActorComponent ActorComponent { get; private set; }
        
        public Vector3 Pos => GetComponent<NavAgent>().Pos;

        public ISerializedObjectData Data
        {
            get
            {
                foreach (var component in _actorData.ActorComponents)
                {
                    var comp = GetComponent(component.Component);
                    var serializedObject = comp as ISerializedObject;
                    if (serializedObject != null)
                    {
                        component.Data = serializedObject.Data;
                    }
                }
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
            var actorManager = (ActorManager)game.Context.GetRuntimeObject(actorManagerDef);
            actorManager.AddActor(this);

            var addedComponents = _actorData.ActorComponents.Select(x => {
                var comp = x.Component.CreateActorComponent(this);
                _components[x.Component] = comp;
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
                component.Value.DeInit();
            }
        }

        public T GetComponent<T>() where T : IActorComponent
        {
            return _components.Values.OfType<T>().FirstOrDefault();
        }

        public IActorComponent GetComponent(ActorComponentDef actorComponentDef)
        {
            return _components[actorComponentDef];
        }
    }
}
