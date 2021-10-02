using Base.Actors;
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

        public void DoUpdate()
        {
            var messagesSystem = MessagesSystem.MessagesSystem.GetMessagesSystemFromContext();
            var messages = messagesSystem.GetMessages(AnimationsComponentDef.NavigationMessages);
            
            messages = messages.Where(x => x.Sender == Actor).ToList();
            if (messages.Any(x => (x.Data as string) == "moving")) {
                Animator.SetInteger("State", (int)AnimatorState.Running);
            }
            if (messages.Any(x => (x.Data as string) == "staying")) {
                Animator.SetInteger("State", (int)AnimatorState.Idle);
            }
        }
    }
}
