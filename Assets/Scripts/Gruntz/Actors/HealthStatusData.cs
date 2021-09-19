using Gruntz.Status;
using System;

namespace Gruntz.Actors
{
    [Serializable]
    public class HealthStatusData : StatusData
    {
        public float Health;
        public float MaxHealth;
    }
}
