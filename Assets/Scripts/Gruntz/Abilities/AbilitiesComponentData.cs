using Base;
using System;
using System.Collections.Generic;

namespace Gruntz.Abilities
{
    [Serializable]
    public class AbilitiesComponentData : ISerializedObjectData
    {
        [Serializable]
        public class AbilityUsageRecord
        {
            public DefRef<HitAbilityDef> Ability;
            public float Downtime;

            [NonSerialized]
            public float LastUsage = float.NegativeInfinity;
        }

        public List<AbilityUsageRecord> AbilitiesUsage = new List<AbilityUsageRecord>();
    }
}
