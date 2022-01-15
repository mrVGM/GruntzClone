using Base;
using Gruntz.Items;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace Gruntz.UI
{
    public class NotificationsDef : Def
    {
        [Serializable]
        public class Notification
        {
            public ItemDef Item;
            [TextArea]
            public string Description;
            public string VideoName;
            public LevelDef LevelDef;
        }

        public Notification[] Notifications;
    }
}
