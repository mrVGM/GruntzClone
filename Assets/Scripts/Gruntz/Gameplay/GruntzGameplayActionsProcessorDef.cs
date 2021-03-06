using Base;
using Base.Actors;
using Base.Gameplay;
using Base.MessagesSystem;
using Base.Navigation;
using Base.Status;
using Gruntz.Actors;
using Gruntz.Gameplay.Actions;
using Gruntz.Statuses;
using Gruntz.SwitchState;
using Gruntz.Team;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using System.Collections.Generic;
using System.Linq;
using Gruntz.Projectile;
using Gruntz.Pushback;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Gruntz.Gameplay
{
    public class GruntzGameplayActionsProcessorDef : GameplayActionsProcessorDef
    {
        public ActorTemplateDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;
        public ActorTemplateDef MaterialActor;
        public MessagesBoxTagDef UnitControllerMessagesBox;
        public BeingPushedNavAgentControllerDef BeingPushedNavAgentControllerDef;

        ProcessAction[] _processActions = null;
        protected override ProcessAction[] ProccessActions
        {
            get
            {
                if (_processActions == null) {
                    _processActions = new ProcessAction[] {
                        ProcessKillActions,
                        ProcessDamageActions,
                        ProcessAddStatusAction,
                        ProcessRemoveStatusAction,
                        ProcessRemoveArrowStatusAction,
                        ProcessActorPushedAction,
                        ProcessChangeActorNavObstaclesAction,
                        ProcessOverrideActorControllerAction,
                        ProcessRedirectActorAction,
                        ProcessRedirectBallActorAction,
                        ProcessSetInitialDestinationAction,
                        ProcessSwitchStateAction,
                        ProcessCollectMaterialAction,
                        ProcessSpawnActorAction,
                    };
                }
                return _processActions;
            }
        }

        private ProcessResult ProcessKillActions(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                List<Actor> dead = new List<Actor>();
                foreach (var action in actions)
                {
                    if (dead.Contains(action.Actor))
                    {
                        continue;
                    }

                    var killAction = action as KillActorAction;
                    if (killAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    var statusComponent = action.Actor.GetComponent<StatusComponent>();
                    var healthStatus = statusComponent.GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
                    if (healthStatus != null)
                    {
                        var healthStatusData = healthStatus.StatusData as HealthStatusData;
                        healthStatusData.Health = 0;
                    }

                    Vector3 pos = killAction.Actor.Pos;
                    var deadActorData = killAction.Actor.Data as ActorData;
                    action.Actor.Deinit();

                    if (killAction.Reason != KillActorAction.DeathReason.ProjectileDestruction) {
                        var graveActor = ActorDeployment.DeployActorFromTemplate(GraveDef, pos);
                        GraveDef.ProcessActor(graveActor);
                        statusComponent = graveActor.GetComponent<StatusComponent>();
                        var actorInstanceStatusData = ActorHolderStatusDef.Data as ActorInstanceHolderStatusData;
                        actorInstanceStatusData.ActorData = deadActorData;
                        statusComponent.AddStatus(actorInstanceStatusData.CreateStatus());
                    }

                    dead.Add(action.Actor);

                    if (killAction.Reason == KillActorAction.DeathReason.Damage) {
                        var nav = Navigation.GetNavigationFromContext();
                        Vector3 snappedPos = nav.Map.SnapPosition(pos);
                        ActorDeployment.DeployActorFromTemplate(MaterialActor, snappedPos);
                    }
                }
            }

            return new ProcessResult {
                ProcessedActions = processActions().ToList(),
                Dirty = dirty
            };
        }

        private ProcessResult ProcessDamageActions(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                var accumulatedDamage = from damageAction in 
                                (from action in actions
                                where action is DamageActorAction
                                select action as DamageActorAction)
                            group damageAction by damageAction.Actor;

                foreach (var accum in accumulatedDamage)
                {
                    dirty = true;
                    var actor = accum.Key;
                    var myTeam = actor.GetComponent<TeamComponent>();
                    var statusComponent = actor.GetComponent<StatusComponent>();
                    var healthStatus = statusComponent.GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
                    var healthStatusData = healthStatus.StatusData as HealthStatusData;
                    float health = healthStatusData.Health;
                    health -= accum.Sum(x => x.DamageValue);
                    healthStatusData.Health = Mathf.Max(0, health);
                    if (health <= 0) {
                        yield return new KillActorAction { Actor = actor, Reason = KillActorAction.DeathReason.Damage };
                    }
                    else {
                        var maxDamage = accum.Where(x => {
                            var team = x.DamageDealer.GetComponent<TeamComponent>();
                            if (team == null) {
                                return false;
                            }
                            if (team.UnitTeam == TeamComponent.Team.Unknown) {
                                return false;
                            }
                            return myTeam.UnitTeam != team.UnitTeam;
                        }).OrderByDescending(x => x.DamageValue).FirstOrDefault();

                        if (maxDamage != null) {
                            var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                            messagesSystem.SendMessage(UnitControllerMessagesBox, MainUpdaterUpdateTime.Update,
                                this,
                                new UnitControllerInstruction {
                                    Unit = actor,
                                    Executable = UnitController.Utils.GetAttackInstruction(actor, maxDamage.DamageDealer)
                                });
                        }
                    }
                }

                foreach (var action in actions)
                {
                    if (action is DamageActorAction)
                    {
                        continue;
                    }
                    yield return action;
                }
            }
            return new ProcessResult
            {
                ProcessedActions = processActions().ToList(),
                Dirty = dirty,
            };
        }
        
        private ProcessResult ProcessChangeActorNavObstaclesAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var changeActorNavObstaclesAction = action as ChangeActorNavObstaclesAction;
                    if (changeActorNavObstaclesAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    dirty = true;

                    var navAgent = changeActorNavObstaclesAction.Actor.GetComponent<NavAgent>();
                    var navAgentData = navAgent.Data as NavAgentData;
                    if (changeActorNavObstaclesAction.Disable)
                    {
                        navAgentData.DisabledObstacleLayers.AddRange(changeActorNavObstaclesAction.DisableNavObstaclesStatus.DisabledObstacleLayers);
                    }
                    else
                    {
                        foreach (string layer in changeActorNavObstaclesAction.DisableNavObstaclesStatus.DisabledObstacleLayers)
                        {
                            navAgentData.DisabledObstacleLayers.Remove(layer);
                        }
                    }
                }
            }

            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessOverrideActorControllerAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var overrideActorControllerAction = action as OverrideActorControllerAction;
                    if (overrideActorControllerAction == null)
                    {
                        yield return action;
                        continue;
                    }
                    dirty = true;

                    var unitController = overrideActorControllerAction.Actor.GetComponent<UnitController.UnitController>();
                    if (overrideActorControllerAction.Restore)
                    {
                        unitController.RemoveMessagesBoxTag(overrideActorControllerAction.MessagesBoxTagDef);
                    }
                    else
                    {
                        unitController.PushMessagesBoxTag(overrideActorControllerAction.MessagesBoxTagDef);
                    }
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessRedirectActorAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var redirectActorAction = action as RedirectActorAction;
                    if (redirectActorAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    dirty = true;

                    var navAgent = redirectActorAction.Actor.GetComponent<NavAgent>();
                    navAgent.Target = new SimpleNavTarget { Target = redirectActorAction.Destination };
                }
            }

            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessRedirectBallActorAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var redirectBallAction = action as RedirectBallActorAction;
                    if (redirectBallAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    dirty = true;

                    var navAgent = redirectBallAction.Actor.GetComponent<NavAgent>();
                    navAgent.Target = new SimpleNavTarget { Target = redirectBallAction.Destination };
                    var statusComponent = redirectBallAction.Actor.GetComponent<StatusComponent>();
                    statusComponent.RemoveStatus(redirectBallAction.StatusToRemove);
                }
            }

            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessSetInitialDestinationAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var setInitialDestinationAction = action as SetInitialDestinationAction;
                    if (setInitialDestinationAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    dirty = true;

                    var statusComponent = setInitialDestinationAction.Actor.GetComponent<StatusComponent>();
                    var navAgent = setInitialDestinationAction.Actor.GetComponent<NavAgent>();
                    navAgent.Target = new SimpleNavTarget { Target = setInitialDestinationAction.Destination };
                    statusComponent.RemoveStatus(setInitialDestinationAction.StatusToRemove);
                }
            }

            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessSwitchStateAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var switchStateAction = action as SwitchStateAction;
                    if (switchStateAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    dirty = true;

                    var switchStateComponent = switchStateAction.Actor.GetComponent<SwitchStateComponent>();
                    IEnumerator<StatusDef> allStatesLooped()
                    {
                        var def = switchStateComponent.SwitchStateComponentDef;
                        int index = 0;
                        while (true)
                        {
                            index %= def.StateStatuses.Length;
                            var cur = switchStateComponent.SwitchStateComponentDef.StateStatuses[index++];
                            yield return cur;
                        }
                    }

                    var allStates = allStatesLooped();
                    allStates.MoveNext();

                    var curState = switchStateComponent.GetCurrentState();
                    while (allStates.Current != curState)
                    {
                        allStates.MoveNext();
                    }

                    allStates.MoveNext();
                    switchStateComponent.SetCurrentState(allStates.Current);
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessCollectMaterialAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var collectMaterialAction = action as CollectMaterialAction;
                    if (collectMaterialAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    var collectedMaterialManager = CollectedMaterialManager.CollectedMaterialManager.GetCollectedMaterialManager();
                    collectedMaterialManager.MaterialCollected();

                    dirty = true;
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessSpawnActorAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var spawnActorAction = action as SpawnActorAction;
                    if (spawnActorAction == null) {
                        yield return action;
                        continue;
                    }

                    var actor = ActorDeployment.DeployActorFromTemplate(spawnActorAction.Template, spawnActorAction.Pos);
                    var teamComponent = actor.GetComponent<TeamComponent>();
                    teamComponent.UnitTeam = TeamComponent.Team.Player;
                    var materialManager = CollectedMaterialManager.CollectedMaterialManager.GetCollectedMaterialManager();
                    materialManager.ActorSpawn();
                    dirty = true;
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResult ProcessActorPushedAction(IEnumerable<IGameplayAction> actions)
        {
            void push(Vector3 direction, NavAgent navAgent)
            {
                var navigation = Navigation.GetNavigationFromContext();
                var map = navigation.Map;
                var snapped = map.SnapPosition(navAgent.Pos);
                var neighbours = map.GetNeighbours(snapped);
                neighbours = neighbours.OrderByDescending(x => {
                    Vector3 neighbourDir = (x - snapped).normalized;
                    return Vector3.Dot(neighbourDir, direction);

                });
                navAgent.Controller = BeingPushedNavAgentControllerDef.CreateController(navAgent, neighbours.FirstOrDefault(), snapped);
            }
            
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var pushActorAction = action as PushActorAction;
                    if (pushActorAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    var unitController = pushActorAction.Actor.GetComponent<UnitController.UnitController>();
                    unitController.CancelInstructions();
                    
                    var navAgent = pushActorAction.Actor.GetComponent<NavAgent>();
                    var projectile = pushActorAction.ProjectileActor.GetComponent<ProjectileComponent>();
                    var projectileComponentData = projectile.Data as ProjectileComponentData;
                    push(((Vector3)projectileComponentData.EndPoint - (Vector3)projectileComponentData.StartPoint).normalized, navAgent);
                    
                    dirty = true;
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }
        
        private ProcessResult ProcessAddStatusAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions() {
                foreach (var action in actions) {
                    var addStatusAction = action as AddStatusAction;
                    if (addStatusAction == null) {
                        yield return action;
                        continue;
                    }

                    var statusComponent = addStatusAction.Actor.GetComponent<StatusComponent>();
                    statusComponent.AddStatus(addStatusAction.Status);
                    
                    dirty = true;
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }
        
        private ProcessResult ProcessRemoveStatusAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions() {
                foreach (var action in actions) {
                    var removeStatusAction = action as RemoveStatusAction;
                    if (removeStatusAction == null) {
                        yield return action;
                        continue;
                    }

                    var statusComponent = removeStatusAction.Actor.GetComponent<StatusComponent>();
                    statusComponent.RemoveStatus(removeStatusAction.Status);
                    
                    dirty = true;
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }
        private ProcessResult ProcessRemoveArrowStatusAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions() {
                foreach (var action in actions) {
                    var removeArrowStatusAction = action as RemoveArrowStatusAction;
                    if (removeArrowStatusAction == null) {
                        yield return action;
                        continue;
                    }

                    var arrowStatus = removeArrowStatusAction.ArrowStatus;
                    var statusComponent = removeArrowStatusAction.Actor.GetComponent<StatusComponent>();
                    
                    var disableNavObstaclesStatuses = statusComponent
                        .GetStatuses(x => x.StatusData is DisableNavObstaclesStatusData)
                        .Where(x => {
                            var data = x.StatusData as DisableNavObstaclesStatusData;
                            return data.AssociatedStatusId == arrowStatus.StatusData.StatusId;
                        });

                    var overrideActorControllerStatuses = statusComponent
                        .GetStatuses(x => x.StatusData is OverrideActorControllerStatusData)
                        .Where(x => {
                            var data = x.StatusData as OverrideActorControllerStatusData;
                            return data.AssociatedStatusId == arrowStatus.StatusData.StatusId;
                        });

                    foreach (var status in disableNavObstaclesStatuses) {
                        yield return new RemoveStatusAction {
                            Actor = removeArrowStatusAction.Actor,
                            Status = status,
                        };
                    }
                    foreach (var status in overrideActorControllerStatuses) {
                        yield return new RemoveStatusAction {
                            Actor = removeArrowStatusAction.Actor,
                            Status = status,
                        };
                    }
                    yield return new RemoveStatusAction {
                        Actor = removeArrowStatusAction.Actor,
                        Status = arrowStatus,
                    };
                    
                    
                    dirty = true;
                }
            }
            return new ProcessResult { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }
    }
}
