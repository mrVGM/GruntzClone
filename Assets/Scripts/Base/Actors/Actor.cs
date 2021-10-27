using Base.Navigation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Actors
{
    public class Actor : ISerializedObject
    {
        ActorData _actorData;
        Dictionary<ActorComponentDef, IActorComponent> _components { get; } = new Dictionary<ActorComponentDef, IActorComponent>();
        public ActorComponent ActorComponent { get; private set; }

        public bool IsInPlay
        {
            get
            {
                var actorManager = ActorManager.GetActorManagerFromContext();
                return actorManager.Actors.Contains(this);
            }
        }
        
        public Vector3 Pos
        {
            get
            {
                var navAgent = GetComponent<NavAgent>();
                if (navAgent != null)
                {
                    return navAgent.Pos;
                }

                return ActorComponent.transform.position;
            }
        }

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
            ActorComponent.gameObject.SetActive(true);
        }

        public void Deinit()
        {
            foreach (var component in _components)
            {
                component.Value.DeInit();
            }
            var actorManager = ActorManager.GetActorManagerFromContext();
            actorManager.RemoveActor(this);
            ActorComponent.gameObject.SetActive(false);
            ActorComponent.gameObject.name = $"Dead - {ActorComponent.gameObject.name}";
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
