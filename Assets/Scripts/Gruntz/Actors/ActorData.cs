using Gruntz.UserInteraction.UnitController;
using System;

namespace Gruntz.Actors
{
    [Serializable]
    public class ActorData
    {
        public UnitControllerDef UnitControllerDef;
        public ActorComponentDef[] ActorComponents;
    }
}
