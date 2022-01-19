using Base;
using Base.Actors;
using Gruntz.Team;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.AI
{
    public class AIBlackboard : IContextObject, IOrderedUpdate
    {
        public struct ActorInfo
        {
            public Actor Actor;
            public Vector3 Pos;
            public Team.TeamComponent.Team Team;
        }

        public AIBlackboardDef AIBlackboardDef;
        public List<ActorInfo> ActorInfos;

        public ExecutionOrderTagDef OrderTagDef => AIBlackboardDef.AIExecutionOrderTagDef;

        public void DisposeObject()
        {
            var game = Game.Instance;
            game.MainUpdater.UnRegisterUpdatable(this);
        }

        public AIBlackboard(AIBlackboardDef def)
        {
            var game = Game.Instance;
            game.MainUpdater.RegisterUpdatable(this);
        }

        public static AIBlackboard GetAIBlackboardFromContext()
        {
            var game = Game.Instance;
            var def = game.DefRepositoryDef.AllDefs.OfType<AIBlackboardDef>().FirstOrDefault();

            return def.CreateRuntimeInstance() as AIBlackboard;
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            var actorManager = ActorManager.GetActorManagerFromContext();
            ActorInfos = actorManager.Actors.Select(x => {
                var info = new ActorInfo { Actor = x, Pos = x.Pos, Team = TeamComponent.Team.Unknown };
                var teamComponent = x.GetComponent<TeamComponent>();
                if (teamComponent != null) {
                    info.Team = teamComponent.UnitTeam;
                }
                return info;
            }).ToList();
        }
    }
}
