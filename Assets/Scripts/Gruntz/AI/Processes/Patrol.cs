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
        public MessagesBoxTagDef MessagesBoxTag;
        public float StayBeforeMovingOn;

        protected override IEnumerator<object> Crt()
        {
            var behaviourTags = CommonAIBehaviourTagsDef.BehaviourTagsDef;
            var possessedActorTag = behaviourTags.PossessedActor;
            var possessedActor = context.GetItem(possessedActorTag) as Actor;
            
            IEnumerator<Vector3> patrolPointsEnumerator()
            {
                int index = 0;
                while (true) {
                    yield return PatrolPoints[index].position;
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
            
            while (true) {
                points.MoveNext();
                var cur = points.Current;
                
                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                var moveToPositionIntruction = new MoveToPosition { Target = new NavAgent.SimpleNavTarget { Target = cur } };
                
                messagesSystem.SendMessage(
                    MessagesBoxTag,
                    MainUpdaterUpdateTime.Update,
                    this,
                    new UnitControllerInstruction
                    {
                        Unit = possessedActor,
                        Executable = moveToPositionIntruction,
                    });

                var staying = timeStaying();

                do {
                    yield return null;
                    staying.MoveNext();
                } while (staying.Current < StayBeforeMovingOn);
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
