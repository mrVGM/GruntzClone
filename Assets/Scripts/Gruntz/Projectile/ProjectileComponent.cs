using Base;
using Base.Actors;
using Base.Gameplay;
using Gruntz.Gameplay;
using System.Linq;
using UnityEngine;

namespace Gruntz.Projectile
{
    public class ProjectileComponent : IActorComponent, ISerializedObject, IOrderedUpdate
    {
        public ProjectileComponentDef ProjectileComponentDef { get; }
        public Actor Actor { get; }

        private ProjectileComponentData _projectileComponentData;
        public ISerializedObjectData Data
        {
            get
            {
                if (_projectileComponentData == null) {
                    _projectileComponentData = new ProjectileComponentData { LifeTime = 0, StartPoint = -1000.0f * Vector3.down, EndPoint = -1000.0f * Vector3.down + Vector3.right };
                }
                return _projectileComponentData;
            }
            set
            {
                _projectileComponentData = value as ProjectileComponentData;
                _parabolaPoints = ProjectileComponentDef.ParabolaSettings
                    .GetParabolaPoints(_projectileComponentData.StartPoint, _projectileComponentData.EndPoint).ToArray();
                PlaceProjectile();
            }
        }

        public ExecutionOrderTagDef OrderTagDef
        {
            get
            {
                var game = Game.Instance;
                var repo = game.DefRepositoryDef;
                return repo.AllDefs.OfType<ProjectileExecutionOrderTagDef>().FirstOrDefault();
            }
        }

        Vector3[] _parabolaPoints;

        public ProjectileComponent(Actor actor, ProjectileComponentDef projectileComponentDef)
        {
            Actor = actor;
            ProjectileComponentDef = projectileComponentDef;
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
            var mainUpdater = game.MainUpdater;
            mainUpdater.RegisterUpdatable(this);
        }

        private void PlaceProjectile()
        {
            Actor.ActorComponent.transform.position = GetProjectilePos();
        }

        private Vector3 GetProjectilePos()
        {
            float dist = _projectileComponentData.LifeTime * ProjectileComponentDef.Speed;
            for (int i = 0; i < _parabolaPoints.Length - 1; ++i)
            {
                Vector3 p1 = _parabolaPoints[i];
                Vector3 p2 = _parabolaPoints[i + 1];

                Vector3 offset = p2 - p1;
                offset.y = 0;
                float d = offset.magnitude;
                if (d > dist) {
                    float c = dist / d;
                    return (1 - c) * p1 + c * p2;
                }

                dist -= d;
            }

            return _parabolaPoints[_parabolaPoints.Length - 1];
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            _projectileComponentData.LifeTime += Time.fixedDeltaTime;
            PlaceProjectile();
            Vector3 pos = GetProjectilePos();
            if ((pos - _parabolaPoints[_parabolaPoints.Length - 1]).magnitude < 0.00001f) {
                var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
                gameplayManager.HandleGameplayEvent(new DestroyProjectileGameplayEvent { ProjectileActor = Actor });
            }
        }
    }
}
