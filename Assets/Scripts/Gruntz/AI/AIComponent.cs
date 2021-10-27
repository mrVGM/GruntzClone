using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.Status;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.AI
{
    public class AIComponent : IActorComponent, IOrderedUpdate
    {
        public AIComponentDef AIComponentDef { get; }
        public Actor Actor { get; }

        private IEnumerator<object> _crt { get; }

        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var game = Game.Instance;
                var defRepo = game.DefRepositoryDef;

                var aiExecutionOrderTagDef = defRepo.AllDefs.OfType<AIExecutionOrderTagDef>().FirstOrDefault();
                return aiExecutionOrderTagDef;
            }
        }

        public AIComponent(AIComponentDef aiComponentDef, Actor actor)
        {
            AIComponentDef = aiComponentDef;
            Actor = actor;
            _crt = UpdateCrt();
        }
        public void DeInit()
        {
            var game = Game.Instance;
            var mainUpdater = game.MainUpdater;
            mainUpdater.UnRegisterUpdatable(this);
        }

        public void Init()
        {
            var game = Game.Instance;
            var mainUpdater = game.MainUpdater;;
            mainUpdater.RegisterUpdatable(this);
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            _crt.MoveNext();
        }

        private IEnumerator<object> UpdateCrt()
        {
            var game = Game.Instance;
            var random = game.Random;
            float timeOffset = AIComponentDef.UpdateInterval * (float) random.NextDouble();
            float lastUpdate = Time.time - timeOffset;

            var team = Actor.GetComponent<Team.TeamComponent>();
            var unitController = Actor.GetComponent<UnitController.UnitController>();

            var actorManager = ActorManager.GetActorManagerFromContext();

            void update()
            {
                if (team.UnitTeam != Team.TeamComponent.Team.Enemy) {
                    return;
                }

                if (unitController.UnitControllerState.FightingWith != null) {
                    return;
                }

                var playerActors = actorManager.Actors.Where(x => {
                    var statusComponent = x.GetComponent<StatusComponent>();
                    if (statusComponent.GetStatus(AIComponentDef.RegularActorStatusDef) == null) {
                        return false;
                    }

                    var teamComponent = x.GetComponent<Team.TeamComponent>();
                    return teamComponent.UnitTeam == Team.TeamComponent.Team.Player;
                });

                var closeActors = playerActors.Where(x => (x.Pos - Actor.Pos).magnitude < AIComponentDef.Range).ToList();
                if (!closeActors.Any()) {
                    return;
                }

                int index = Mathf.FloorToInt((float)random.NextDouble() * closeActors.Count());
                var targetActor = closeActors[index];
                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                messagesSystem.SendMessage(AIComponentDef.MessagesBox,
                    MainUpdaterUpdateTime.Update,
                    this, 
                    new UnitControllerInstruction {
                        Unit = Actor,
                        Executable = new AttackUnit(targetActor)
                    });
            }

            IEnumerator<object> intervalUpdate()
            {
                while (true) {
                    if (Time.time - lastUpdate < AIComponentDef.UpdateInterval) {
                        yield return null;
                        continue;
                    }

                    var team = Actor.GetComponent<Team.TeamComponent>();
                    if (team.UnitTeam != Team.TeamComponent.Team.Enemy) {
                        lastUpdate = Time.time;
                        yield return null;
                        continue;
                    }


                    update();
                    lastUpdate = Time.time;
                    yield return null;
                }
            }

            return intervalUpdate();
        }
    }
}
