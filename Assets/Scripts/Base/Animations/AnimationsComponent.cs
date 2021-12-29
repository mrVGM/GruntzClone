using Base.Actors;
using Base.Navigation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Base.Animations
{
    public class AnimationsComponent : IActorComponent, IOrderedUpdate
    {
        private enum AnimatorState
        {
            Idle = 0,
            Running = 1,
            Ability = 2,
        }

        public struct AbilityAnimationInfo
        {
            public Actor Actor;
            public AnimationClip Animation;
        }

        public AnimationsComponentDef AnimationsComponentDef { get; }
        public Actor Actor { get; }
        private Animator Animator { get; }

        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var defRepository = Game.Instance.DefRepositoryDef;
                return defRepository.AllDefs.OfType<AnimationsUpdateOrderTagDef>().FirstOrDefault();
            }
        }

        public AnimationsComponent(Actor actor, AnimationsComponentDef animationsComponentDef)
        {
            Actor = actor;
            Animator = Actor.ActorComponent.GetComponentInChildren<Animator>();
            var controller = Animator.runtimeAnimatorController;
            controller = new AnimatorOverrideController(controller);
            Animator.runtimeAnimatorController = controller;
            AnimationsComponentDef = animationsComponentDef;
        }
        public void DeInit()
        {
            Game.Instance.MainUpdater.UnRegisterUpdatable(this);
        }

        public void Init()
        {
            Game.Instance.MainUpdater.RegisterUpdatable(this);
            Animator.SetInteger("State", (int)AnimatorState.Idle);
            var state = Animator.GetCurrentAnimatorStateInfo(0);
            Animator.Play(state.shortNameHash, 0, (float)Game.Instance.Random.NextDouble());
        }

        private void OverrideAbilityAnimation(AnimationClip animation)
        {
            var overrideController = Animator.runtimeAnimatorController as AnimatorOverrideController;
            var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
            overrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(AnimationsComponentDef.DefaultAbilityAnimation, animation));
            overrideController.ApplyOverrides(overrides);
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            var messagesSystem = MessagesSystem.MessagesSystem.GetMessagesSystemFromContext();
            var messages = messagesSystem.GetMessages(AnimationsComponentDef.NavigationMessages);
            var abilityMessages = messagesSystem.GetMessages(AnimationsComponentDef.AbilityMessages);
            
            messages = messages.Where(x => x.Sender == Actor).ToList();
            if (messages.Any(x => (NavAgent.NavAgentState)x.Data == NavAgent.NavAgentState.Moving)) {
                Animator.SetInteger("State", (int)AnimatorState.Running);
            }
            if (messages.Any(x => (NavAgent.NavAgentState)x.Data == NavAgent.NavAgentState.Statying)) {
                Animator.SetInteger("State", (int)AnimatorState.Idle);
            }

            var abilitiesMessagesForMe = abilityMessages.Select(x => (AbilityAnimationInfo)x.Data).Where(x => x.Actor == Actor);
            foreach (var message in abilitiesMessagesForMe) {
                OverrideAbilityAnimation(message.Animation);
                Animator.SetInteger("State", (int)AnimatorState.Ability);
            }
        }
    }
}
