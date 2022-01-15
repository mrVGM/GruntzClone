using System;
using System.Collections.Generic;
using Base;
using UnityEngine;

namespace Gruntz.TriggerBox
{
    public class NotificationDataBehaviour : MonoBehaviour
    {
        [Serializable]
        public class Notification
        {
            public List<Sprite> ImagesToDisplay = new List<Sprite>();
            public string NotificationText;
            public string VideoName;
            public LevelDef LevelToStart;
        }
        public Notification[] Notifications;
    }
}
