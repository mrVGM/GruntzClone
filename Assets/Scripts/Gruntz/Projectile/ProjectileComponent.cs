using Base;
using Base.Actors;
using Base.Gameplay;
using Base.Status;
using Gruntz.Gameplay;
using Gruntz.Statuses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gruntz.Projectile
{
    public class ProjectileComponent : IActorComponent, ISerializedObject, IOrderedUpdate
    {
        public ProjectileComponentDef ProjectileComponentDef { get; }
        public Actor Actor { get; }

        private ProjectileComponentData _projectileComponentData;
        private float _speed = 0;

        private Actor __ownerActor;
        public Actor OwnerActor
        {
            get
            {
                if (__ownerActor == null) {
                    var game = Game.Instance;
                    var repo = game.DefRepositoryDef;
                    var actorIDStatusDef = repo.AllDefs.OfType<ActorIDStatusDef>().FirstOrDefault();

                    var actorManager = ActorManager.GetActorManagerFromContext();
                    __ownerActor = actorManager.Actors.FirstOrDefault(actor => {
                        var statusComponent = actor.GetComponent<StatusComponent>();
                        var idStatus = statusComponent.GetStatus(actorIDStatusDef);
                        var statusData = idStatus.StatusData as ActorIDStatusData;
                        return statusData.ID == _projectileComponentData.OwnerActorId;
                    });
                }
                return __ownerActor;
            }
        }

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

                Vector3 slopeVector = ProjectileComponentDef.ParabolaSettings.GetSlopeVector(_projectileComponentData.StartPoint, _projectileComponentData.EndPoint);
                Vector3 horizontalVelocity = ProjectileComponentDef.Speed * slopeVector;
                horizontalVelocity.y = 0;
                _speed = horizontalVelocity.magnitude;
                PlaceProjectile(out _, out _);
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

        private void PlaceProjectile(out int prevPoint, out int nextPoint)
        {
            Actor.ActorComponent.transform.position = GetProjectilePos(out prevPoint, out nextPoint);
        }

        private Vector3 GetProjectilePos(out int prevPoint, out int nextPoint)
        {
            float dist = _speed * _projectileComponentData.LifeTime;
            for (int i = 0; i < _parabolaPoints.Length - 1; ++i)
            {
                Vector3 p1 = _parabolaPoints[i];
                Vector3 p2 = _parabolaPoints[i + 1];

                Vector3 offset = p2 - p1;
                offset.y = 0;
                float d = offset.magnitude;
                if (d > dist) {
                    float c = dist / d;
                    prevPoint = i;
                    nextPoint = i + 1;
                    return (1 - c) * p1 + c * p2;
                }

                dist -= d;
            }

            prevPoint = _parabolaPoints.Length - 1;
            nextPoint = _parabolaPoints.Length - 1;
            return _parabolaPoints[_parabolaPoints.Length - 1];
        }

        private IEnumerable<RaycastHit> GetHits(IEnumerable<Vector3> points)
        {
            if (!points.Any()) {
                yield break;
            }
            var pts = points.ToArray();
            for (int i = 0; i < pts.Length - 1; ++i) {
                Vector3 offset = pts[i + 1] - pts[i];
                var ray = new Ray(pts[i], offset);

                var hits = Physics.RaycastAll(pts[i], pts[i + 1] - pts[i]);
                foreach (var hit in hits) {
                    yield return hit;
                }
            }
        }

        public void DoUpdate(MainUpdaterUpdateTime updateTime)
        {
            int p2, p3;
            var pos1 = GetProjectilePos(out _, out p2);
            _projectileComponentData.LifeTime += Time.fixedDeltaTime;
            var pos2 = GetProjectilePos(out p3, out _);

            IEnumerable<Vector3> points()
            {
                if (p3 <= _parabolaPoints.Length / 2) {
                    yield break;
                }
                yield return pos1;
                for (int i = p2; i <= p3; ++i) {
                    yield return _parabolaPoints[i];
                }
                yield return pos2;
            }

            var hits = GetHits(points());

            int prev, next;
            PlaceProjectile(out prev, out next);

            var actorsHit = hits
                .Select(x => x.collider.GetComponent<ActorProxy>())
                .Where(x => x != null)
                .Select(x => x.Actor);

            var gameplayManager = GameplayManager.GetGameplayManagerFromContext();
            foreach (var actor in actorsHit) {
                gameplayManager.HandleGameplayEvent(new ProjectileHitActorGameplayEvent { ProjectileActor = Actor, ActorHit = actor });
            }

            bool shouldDestroy = false;
            if (actorsHit.Any()) {
                shouldDestroy = true;
            }
            if (prev == _parabolaPoints.Length - 1) {
                shouldDestroy = true;
            }

            if (shouldDestroy) {
                gameplayManager.HandleGameplayEvent(new DestroyProjectileGameplayEvent { ProjectileActor = Actor });
            }
        }
    }
}
