using Base;
using Base.Actors;
using Base.Navigation;
using Base.Status;
using Gruntz.Actors;
using Gruntz.Gameplay.Actions;
using Gruntz.Statuses;
using Gruntz.SwitchState;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Base.Navigation.NavAgent;

namespace Gruntz.Gameplay
{
    public class GameplayActionsProcessorDef : Def
    {
        private class ProcessorResult
        {
            public bool Dirty = false;
        }

        public ActorTemplateDef GraveDef;
        public ActorInstanceHolderStatusDef ActorHolderStatusDef;

        private delegate IEnumerable<IGameplayAction> ProcessAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult);
        private ProcessAction[] processActions { get; set; } = null;

        public void ProcessActions(IEnumerable<IGameplayAction> actions)
        {
            if (processActions == null) {
                processActions = new ProcessAction[] {
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
            var result = new ProcessorResult();
            int index = 0;
            while (unprocessed.Any()) {
                result.Dirty = false;
                var cur = processActions[index];
                unprocessed = cur(unprocessed, result);
                ++index;
                if (result.Dirty || index >= processActions.Length) {
                    index = 0;
                }
            }
        }

        private IEnumerable<IGameplayAction> ProcessKillActions(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            List<Actor> dead = new List<Actor>();

            foreach (var action in actions) {
                if (dead.Contains(action.Actor)) {
                    continue;
                }

                var killAction = action as KillActorAction;
                if (killAction == null) {
                    yield return action;
                    continue;
                }

                var statusComponent = action.Actor.GetComponent<StatusComponent>();
                var healthStatus = statusComponent.GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
                if (healthStatus != null) {
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
            }
        }

        private IEnumerable<IGameplayAction> ProcessDamageActions(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            var actorAccumulatedDamage = new Dictionary<Actor, float>();
            var damageActions = actions.OfType<DamageActorAction>();
            foreach (var damageAction in damageActions) {
                if (!actorAccumulatedDamage.TryGetValue(damageAction.Actor, out _)) {
                    actorAccumulatedDamage[damageAction.Actor] = 0;
                }
                actorAccumulatedDamage[damageAction.Actor] += damageAction.DamageValue;
            }

            foreach (var pair in actorAccumulatedDamage) {
                var statusComponent = pair.Key.GetComponent<StatusComponent>();
                var healthStatus = statusComponent.GetStatuses(x => x.StatusData is HealthStatusData).FirstOrDefault();
                var healthStatusData = healthStatus.StatusData as HealthStatusData;
                float health = healthStatusData.Health;
                health -= pair.Value;
                healthStatusData.Health = Mathf.Max(0, health);
                if (health <= 0) {
                    yield return new KillActorAction { Actor = pair.Key, GraveDef = GraveDef, ActorHolderStatusDef = ActorHolderStatusDef };
                }
            }

            foreach (var action in actions) {
                if (action is DamageActorAction) {
                    continue;
                }
                yield return action;
            }
        }

        private IEnumerable<IGameplayAction> ProcessActorArrivedAtArrowDestinationAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            foreach (var action in actions) {
                var arrivedAtDestinationAction = action as ActorArrivedAtArrowDestinationAction;
                if (arrivedAtDestinationAction == null) {
                    yield return action;
                    continue;
                }
                var statusComponent = arrivedAtDestinationAction.Actor.GetComponent<StatusComponent>();
                foreach (var status in arrivedAtDestinationAction.StatusesToRemove) {
                    statusComponent.RemoveStatus(status);
                }
            }
        }

        private IEnumerable<IGameplayAction> ProcessChangeActorNavObstaclesAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            foreach (var action in actions) {
                var changeActorNavObstaclesAction = action as ChangeActorNavObstaclesAction;
                if (changeActorNavObstaclesAction == null) {
                    yield return action;
                    continue;
                }

                var navAgent = changeActorNavObstaclesAction.Actor.GetComponent<NavAgent>();
                var navAgentData = navAgent.Data as NavAgentData;
                if (changeActorNavObstaclesAction.Disable) {
                    navAgentData.DisabledObstacleLayers.AddRange(changeActorNavObstaclesAction.DisableNavObstaclesStatus.DisabledObstacleLayers);
                }
                else {
                    foreach (string layer in changeActorNavObstaclesAction.DisableNavObstaclesStatus.DisabledObstacleLayers) {
                        navAgentData.DisabledObstacleLayers.Remove(layer);
                    }
                }
            }
        }

        private IEnumerable<IGameplayAction> ProcessOverrideActorControllerAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            foreach (var action in actions) {
                var overrideActorControllerAction = action as OverrideActorControllerAction;
                if (overrideActorControllerAction == null) {
                    yield return action;
                    continue;
                }

                var unitController = overrideActorControllerAction.Actor.GetComponent<UnitController.UnitController>();
                if (overrideActorControllerAction.Restore) {
                    unitController.RemoveMessagesBoxTag(overrideActorControllerAction.MessagesBoxTagDef);
                }
                else {
                    unitController.PushMessagesBoxTag(overrideActorControllerAction.MessagesBoxTagDef);
                }
            }
        }

        private IEnumerable<IGameplayAction> ProcessRedirectActorAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            foreach (var action in actions) {
                var redirectActorAction = action as RedirectActorAction;
                if (redirectActorAction == null) {
                    yield return action;
                    continue;
                }

                var navAgent = redirectActorAction.Actor.GetComponent<NavAgent>();
                navAgent.Target = new SimpleNavTarget { Target = redirectActorAction.Destination };
                var statusComponent = redirectActorAction.Actor.GetComponent<StatusComponent>();
                foreach (var status in redirectActorAction.StatusesToAdd) {
                    statusComponent.AddStatus(status);
                }
            }
        }

        private IEnumerable<IGameplayAction> ProcessRedirectBallActorAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            foreach (var action in actions) {
                var redirectBallAction = action as RedirectBallActorAction;
                if (redirectBallAction == null) {
                    yield return action;
                    continue;
                }

                var navAgent = redirectBallAction.Actor.GetComponent<NavAgent>();
                navAgent.Target = new SimpleNavTarget { Target = redirectBallAction.Destination };
                var statusComponent = redirectBallAction.Actor.GetComponent<StatusComponent>();
                statusComponent.RemoveStatus(redirectBallAction.StatusToRemove);
            }
        }

        private IEnumerable<IGameplayAction> ProcessSetInitialDestinationAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            foreach (var action in actions)
            {
                var setInitialDestinationAction = action as SetInitialDestinationAction;
                if (setInitialDestinationAction == null)
                {
                    yield return action;
                    continue;
                }

                var statusComponent = setInitialDestinationAction.Actor.GetComponent<StatusComponent>();
                var navAgent = setInitialDestinationAction.Actor.GetComponent<NavAgent>();
                navAgent.Target = new SimpleNavTarget { Target = setInitialDestinationAction.Destination };
                statusComponent.RemoveStatus(setInitialDestinationAction.StatusToRemove);
            }
        }

        private IEnumerable<IGameplayAction> ProcessSwitchStateAction(IEnumerable<IGameplayAction> actions, ProcessorResult processorResult)
        {
            foreach (var action in actions) {
                var switchStateAction = action as SwitchStateAction;
                if (switchStateAction == null) {
                    yield return action;
                    continue;
                }

                var switchStateComponent = switchStateAction.Actor.GetComponent<SwitchStateComponent>();
                IEnumerator<StatusDef> allStatesLooped()
                {
                    var def = switchStateComponent.SwitchStateComponentDef;
                    int index = 0;
                    while (true) {
                        index %= def.StateStatuses.Length;
                        var cur = switchStateComponent.SwitchStateComponentDef.StateStatuses[index++];
                        yield return cur;
                    }
                }

                var allStates = allStatesLooped();
                allStates.MoveNext();

                var curState = switchStateComponent.GetCurrentState();
                while (allStates.Current != curState) {
                    allStates.MoveNext();
                }

                allStates.MoveNext();
                switchStateComponent.SetCurrentState(allStates.Current);
            }
        }
    }
}
