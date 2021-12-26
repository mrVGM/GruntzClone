using Base.Actors;
using Base.UI;
using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using Base.Navigation;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using UnityEngine;

namespace Gruntz.AI.Processes
{
    public class StopFightingIfOutOfGuardZone : CoroutineProcess
    {
        public Transform GuardZoneCenter;
        public float GuardZoneRange;
        public MessagesBoxTagDef MessagesBoxTagDef;
        
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;

            while ((possessedActor.Pos - GuardZoneCenter.position).magnitude <= GuardZoneRange) {
                yield return null;
            }
            
            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();

            messagesSystem.SendMessage(
                MessagesBoxTagDef,
                MainUpdaterUpdateTime.Update,
                this,
                new UnitControllerInstruction {
                    Unit = possessedActor,
                    Executable = new ClearInstruction()
                });
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
