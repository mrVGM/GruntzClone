using Base;
using Base.Actors;
using Base.MessagesSystem;
using Base.Navigation;
using Base.Status;
using Gruntz.Actors;
using Gruntz.Gameplay.Actions;
using Gruntz.Statuses;
using Gruntz.SwitchState;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Gruntz.Gameplay
{
    public class GameplayActionsProcessorDef : Def
    {
        private class ProcessResultt
        {
            public List<IGameplayAction> ProcessedActions;
            public bool Dirty;
        }

        public ActorTemplateDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;
        public ActorTemplateDef MaterialActor;
        public MessagesBoxTagDef UnitControllerMessagesBox;

        private delegate ProcessResultt ProcessAction(IEnumerable<IGameplayAction> actions);
        private ProcessAction[] _processActions { get; set; } = null;

        public void ProcessActions(IEnumerable<IGameplayAction> actions)
        {
            if (_processActions == null) {
                _processActions = new ProcessAction[] {
                    ProcessKillActions,
                    ProcessDamageActions,
                    ProcessActorArrivedAtArrowDestinationAction,
                    ProcessChangeActorNavObstaclesAction,
                    ProcessOverrideActorControllerAction,
                    ProcessRedirectActorAction,
                    ProcessRedirectBallActorAction,
                    ProcessSetInitialDestinationAction,
                    ProcessSwitchStateAction,
                };
            }
            var unprocessed = actions;
            int index = 0;
            while (index < _processActions.Length) {
                var cur = _processActions[index];
                var res = cur(unprocessed);
                unprocessed = res.ProcessedActions;
                ++index;
                if (res.Dirty) {
                    index = 0;
                }
            }
        }

        private ProcessResultt ProcessKillActions(IEnumerable<IGameplayAction> actions)
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
                    var graveActor = ActorDeployment.DeployActorFromTemplate(GraveDef, pos);
                    GraveDef.ProcessActor(graveActor);
                    statusComponent = graveActor.GetComponent<StatusComponent>();
                    var actorInstanceStatusData = ActorHolderStatusDef.Data as ActorInstanceHolderStatusData;
                    actorInstanceStatusData.ActorData = deadActorData;
                    statusComponent.AddStatus(actorInstanceStatusData.CreateStatus());

                    dead.Add(action.Actor);

                    if (killAction.Reason == KillActorAction.DeathReason.Damage) {
                        var nav = Navigation.GetNavigationFromContext();
                        Vector3 snappedPos = nav.Map.SnapPosition(pos);
                        ActorDeployment.DeployActorFromTemplate(MaterialActor, snappedPos);
                    }
                }
            }

            return new ProcessResultt {
                ProcessedActions = processActions().ToList(),
                Dirty = dirty
            };
        }

        private ProcessResultt ProcessDamageActions(IEnumerable<IGameplayAction> actions)
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
                        var maxDamage = accum.OrderByDescending(x => x.DamageValue).FirstOrDefault();
                        var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                        messagesSystem.SendMessage(UnitControllerMessagesBox, MainUpdaterUpdateTime.Update,
                            this,
                            new UnitControllerInstruction {
                            Unit = actor,
                            Executable = new AttackUnit(maxDamage.DamageDealer)
                        });
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
            return new ProcessResultt
            {
                ProcessedActions = processActions().ToList(),
                Dirty = dirty,
            };
        }

        private ProcessResultt ProcessActorArrivedAtArrowDestinationAction(IEnumerable<IGameplayAction> actions)
        {
            bool dirty = false;
            IEnumerable<IGameplayAction> processActions()
            {
                foreach (var action in actions)
                {
                    var arrivedAtDestinationAction = action as ActorArrivedAtArrowDestinationAction;
                    if (arrivedAtDestinationAction == null)
                    {
                        yield return action;
                        continue;
                    }

                    dirty = true;
                    var statusComponent = arrivedAtDestinationAction.Actor.GetComponent<StatusComponent>();
                    foreach (var status in arrivedAtDestinationAction.StatusesToRemove)
                    {
                        statusComponent.RemoveStatus(status);
                    }
                }
            }
            return new ProcessResultt { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResultt ProcessChangeActorNavObstaclesAction(IEnumerable<IGameplayAction> actions)
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

            return new ProcessResultt { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResultt ProcessOverrideActorControllerAction(IEnumerable<IGameplayAction> actions)
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
            return new ProcessResultt { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResultt ProcessRedirectActorAction(IEnumerable<IGameplayAction> actions)
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
                    var statusComponent = redirectActorAction.Actor.GetComponent<StatusComponent>();
                    foreach (var status in redirectActorAction.StatusesToAdd)
                    {
                        statusComponent.AddStatus(status);
                    }
                }
            }

            return new ProcessResultt { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResultt ProcessRedirectBallActorAction(IEnumerable<IGameplayAction> actions)
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

            return new ProcessResultt { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResultt ProcessSetInitialDestinationAction(IEnumerable<IGameplayAction> actions)
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

            return new ProcessResultt { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }

        private ProcessResultt ProcessSwitchStateAction(IEnumerable<IGameplayAction> actions)
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
            return new ProcessResultt { ProcessedActions = processActions().ToList(), Dirty = dirty };
        }
    }
}
