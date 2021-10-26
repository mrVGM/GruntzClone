using Base.Actors;
using Base.Status;

namespace Gruntz.Team
{
    public class TeamComponentDef : ActorComponentDef
    {
        public StatusDef PlayerTeamStatusDef;
        public StatusDef EnemyTeamStatusDef;

        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new TeamComponent(this, actor);
        }
    }
}
