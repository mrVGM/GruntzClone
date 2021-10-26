using Base.Actors;
using Base.Status;
using Gruntz.SwitchState;
using System.Collections.Generic;

namespace Gruntz.Gameplay.Actions
{
    public class SwitchStateAction : IGameplayAction
    {
        public Actor Actor { get; set; }
    }
}
