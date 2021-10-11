using Base.Actors;
using UnityEngine;

namespace Base.Navigation
{
    public class SimplePositionComponent : IActorComponent, ISerializedObject
    {
        public SimplePositionComponentDef SimplePositionComponentDef { get; }
        public Actor Actor { get; }
        private SimplePositionComponentData _simplePositionComponentData;

        public ISerializedObjectData Data
        {
            get
            {
                if (_simplePositionComponentData == null) {
                    _simplePositionComponentData = new SimplePositionComponentData();
                }
                _simplePositionComponentData.Position = Actor.Pos;
                return _simplePositionComponentData;
            }
            set
            {
                _simplePositionComponentData = value as SimplePositionComponentData;
                Pos = _simplePositionComponentData.Position;
            }
        }

        public SimplePositionComponent(SimplePositionComponentDef simplePositionComponentDef, Actor actor)
        {
            SimplePositionComponentDef = simplePositionComponentDef;
            Actor = actor;
        }

        public Vector3 Pos
        {
            set
            {
                _simplePositionComponentData.Position = value;
                Actor.ActorComponent.transform.position = value;
            }
        }

        public void Init()
        {
        }
        public void DeInit()
        {
        }
    }
}
