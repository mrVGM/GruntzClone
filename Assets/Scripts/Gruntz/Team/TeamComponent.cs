using Base.Actors;
using Base.Status;
using System.Linq;

namespace Gruntz.Team
{
    public class TeamComponent : IActorComponent
    {
        public enum Team
        {
            Unknown,
            Player,
            Enemy
        }
        public TeamComponentDef TeamComponentDef { get; }
        public Actor Actor { get; }

        public Team _team;
        public Team UnitTeam
        {
            get
            {
                var statusComponent = Actor.GetComponent<StatusComponent>();
                var status = statusComponent.GetStatus(TeamComponentDef.PlayerTeamStatusDef);
                if (status != null) {
                    return Team.Player;
                }
                status = statusComponent.GetStatus(TeamComponentDef.EnemyTeamStatusDef);
                if (status != null) {
                    return Team.Enemy;
                }
                return Team.Unknown;
            }
            set
            {
                var statusComponent = Actor.GetComponent<StatusComponent>();
                var statuses = statusComponent
                    .GetStatuses(x => x.StatusDef == TeamComponentDef.PlayerTeamStatusDef || x.StatusDef == TeamComponentDef.EnemyTeamStatusDef)
                    .ToList();

                foreach (var status in statuses) {
                    statusComponent.RemoveStatus(status);
                }
                if (value == Team.Player) {
                    statusComponent.AddStatus(TeamComponentDef.PlayerTeamStatusDef.Data.CreateStatus());
                }
                else {
                    statusComponent.AddStatus(TeamComponentDef.EnemyTeamStatusDef.Data.CreateStatus());
                }
            }
        }

        public TeamComponent(TeamComponentDef teamComponentDef, Actor actor)
        {
            TeamComponentDef = teamComponentDef;
            Actor = actor;
        }
        public void DeInit()
        {
        }

        public void Init()
        {
        }
    }
}
