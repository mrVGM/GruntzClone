using System;
using Base;
using Base.Actors;

namespace Gruntz.Actors
{
    [Serializable]
    public class ActorDeployDef : Def
    {
        public ActorDef ActorDef;
        public ActorComponentDef[] ActorComponents;
    }
}
