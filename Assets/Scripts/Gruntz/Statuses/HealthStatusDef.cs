using Base.Status;

namespace Gruntz.Statuses
{
    public class HealthStatusDef : StatusDef
    {
        public float InitialHealth;
        public float InitialMaxHealth;
        protected override StatusData StatusData
        {
            get
            {
                return new HealthStatusData {
                    Health = InitialHealth,
                    MaxHealth = InitialMaxHealth
                };
            }
        }
    }
}
