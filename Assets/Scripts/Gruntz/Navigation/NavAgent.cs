using System.Linq;
using Base;
using UnityEngine;

namespace Gruntz.Navigation
{
    public class NavAgent : MonoBehaviour, IOrderedUpdate
    {
        public Vector3 Target;
        public IOccupiedPosition OccupiedPosition;

        public NavAgentDef NavAgentDef;
        private NavAgentDef.NavAgentStats navStats = null;

        public NavAgentDef.NavAgentStats NavStats => navStats ?? NavAgentDef.NavStats;

        public ExecutionOrderTagDef OrderTagDef 
        {
            get
            {
                var game = Game.Instance;
                var defRepo = game.DefRepositoryDef;
                return defRepo.AllDefs.OfType<NavigationRequestOrderTagDef>().FirstOrDefault();
            }
        }

        public void DoUpdate()
        {
            var game = Game.Instance;
            var navDef = game.DefRepositoryDef.AllDefs.OfType<NavigationDef>().FirstOrDefault();
            var context = game.GetComponent<Context>();
            var navigation = context.GetRuntimeObject(navDef) as Navigation;

            var request = new MoveRequest
            {
                CurrentPos = transform.position,
                CurrentDirection = transform.rotation * Vector3.forward,
                TargetPos = Target,
                MoveSpeed = NavStats.Speed,
                OccupiedPosition = OccupiedPosition,
                MoveResultCallback = Move
            };

            navigation.MakeMoveRequest(request);
        }

        void Start() 
        {
            var game = Game.Instance;
            var mainUpdater = game.GetComponent<MainUpdater>();
            mainUpdater.RegisterUpdatable(this);
        }

        private void Move(MoveRequestResult moveRequestResult) 
        {
            transform.position = moveRequestResult.PositionToMove;
            OccupiedPosition = moveRequestResult.OccupiedPosition;
            if (moveRequestResult.Direction.sqrMagnitude > Navigation.Eps)
            {
                transform.rotation = Quaternion.LookRotation(moveRequestResult.Direction);
            }
        }
    }
}
