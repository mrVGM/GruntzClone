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
    public class Patrol : CoroutineProcess
    {
        public Transform[] PatrolPoints;
        public float StayBeforeMovingOn;
        public ProcessContextTagDef InstructionsAccumulatedDef;

        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            
            IEnumerator<Transform> patrolPointsEnumerator()
            {
                int index = 0;
                while (true) {
                    yield return PatrolPoints[index];
                    ++index;
                    while (index >= PatrolPoints.Length) {
                        index -= PatrolPoints.Length;
                    }
                }
            }

            IEnumerator<float> timeStaying()
            {
                Vector3 curPos = possessedActor.Pos;
                float curPosDetected = Time.time;

                while (true) {
                    Vector3 offset = possessedActor.Pos - curPos;
                    if (offset.sqrMagnitude > 0.1f) {
                        curPos = possessedActor.Pos;
                        curPosDetected = Time.time;
                    }

                    yield return Time.time - curPosDetected;
                }
            }

            var points = patrolPointsEnumerator();
            var closest = PatrolPoints.OrderBy(x => (possessedActor.Pos - x.position).sqrMagnitude).First();
            points.MoveNext();
            while (points.Current != closest) {
                points.MoveNext();
            }

            while (true) {
                var cur = points.Current;
                
                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                var moveToPositionInstruction = new MoveToPosition { Target = new NavAgent.SimpleNavTarget { Target = cur.position } };

                List<IUnitExecutable> instructionList =
                    context.GetItem(InstructionsAccumulatedDef) as List<IUnitExecutable>;
                if (instructionList == null) {
                    instructionList = new List<IUnitExecutable>();
                    context.PutItem(InstructionsAccumulatedDef, instructionList);
                }
                instructionList.Add(moveToPositionInstruction);
                var staying = timeStaying();

                do {
                    yield return null;
                    staying.MoveNext();
                } while (staying.Current < StayBeforeMovingOn);
                
                points.MoveNext();
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
