using System;

namespace Base.Actors
{
    [Serializable]
    public class ActorData : ISerializedObjectData
    {
        [Serializable]
        public class Components
        {
            public DefRef<ActorComponentDef> _component;
            public ActorComponentDef Component => _component;
            public ISerializedObjectData Data;
        }

        public ActorComponent ActorPrefab => ((ActorDef)ActorDef).ActorPrefab;
        public DefRef<ActorDef> ActorDef;
        public Components[] ActorComponents;
    }
}
