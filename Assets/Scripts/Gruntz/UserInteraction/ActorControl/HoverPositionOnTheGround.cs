using System.Collections.Generic;
using System.Linq;
using Base;
using Base.MessagesSystem;
using UnityEngine;
using Utils;

namespace Gruntz.UserInteraction.ActorControl
{
    public class HoverPositionOnTheGround : Process
    {
        public MessagesBoxTagDef HitResultsMessageTag;
        public GameObject GroundSelectionMarker;

        bool isRunning = false;

        public override bool IsFinished => isTermninated && !isRunning;

        public override void DoUpdate()
        {
            if (!isTermninated)
            {
                isRunning = true;
            }
            else
            {
                GroundSelectionMarker.SetActive(false);
                isRunning = false;
            }

            var game = Game.Instance;

            var messageSystemTag = game.DefRepositoryDef.AllDefs.OfType<MessagesSystemDef>().FirstOrDefault();
            var messagesSystem = game.Context.GetRuntimeObject(messageSystemTag) as MessagesSystem;
            Debug.Log($"MessagesSystem: {messagesSystem}");
            var hits = messagesSystem.GetMessages(HitResultsMessageTag).FirstOrDefault().Data as IEnumerable<RaycastHit>;
            var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == UnityLayers.Floor);
            if (floorHit.collider == null) 
            {
                GroundSelectionMarker.SetActive(false);
                return;
            }

            Vector3 pos = floorHit.point;
            Vector3 center = 0.5f * Vector3.right + 0.5f * Vector3.forward;
            pos -= center;
            pos.x = Mathf.Round(pos.x);
            pos.z = Mathf.Round(pos.z);
            pos += center;
            GroundSelectionMarker.transform.position = pos;
        }
    }
}
