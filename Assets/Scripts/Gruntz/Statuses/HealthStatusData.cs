using Base.Status;
using System;

namespace Gruntz.Statuses
{
    [Serializable]
    public class HealthStatusData : StatusData
    {
        public float Health;
        public float MaxHealth;
    }
}
