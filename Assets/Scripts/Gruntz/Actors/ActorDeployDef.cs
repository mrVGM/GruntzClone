using System;
using Base;

namespace Gruntz.Actors
{
    [Serializable]
    public class ActorDeployDef : Def
    {
        public ActorDef ActorDef;
        public ActorComponentDef[] ActorComponents;
    }
}
