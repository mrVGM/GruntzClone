using Base;
using Gruntz.Items;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace Gruntz.UI
{
    public class TutorialNotificationsDef : Def
    {
        [Serializable]
        public class Notification
        {
            public ItemDef Item;
            [TextArea]
            public string Description;
            public string VideoName;
        }

        public Notification[] Notifications;
    }
}
