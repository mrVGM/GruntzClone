using Base;
using Gruntz.Abilities;
using UnityEngine;

namespace Gruntz.Items
{
    public class ItemDef : Def
    {
        public Sprite Icon;
        public GameObject Prefab;
        public AbilityDef[] Abilities;
        [TextArea]
        public string Description;
    }
}
