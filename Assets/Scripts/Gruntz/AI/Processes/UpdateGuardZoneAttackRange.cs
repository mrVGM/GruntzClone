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
    public class UpdateGuardZoneAttackRange : CoroutineProcess
    {
        public Transform GuardZoneCenter;
        public ProcessContextTagDef AttackRangeTagDef;
        public float GuardZoneRange;
        public float AttackRange;
        
        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            
            context.PutItem(AttackRangeTagDef, 0.0f);

            while ((possessedActor.Pos - GuardZoneCenter.position).magnitude > GuardZoneRange) {
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            context.PutItem(AttackRangeTagDef, AttackRange);
            yield break;
        }
    }
}
