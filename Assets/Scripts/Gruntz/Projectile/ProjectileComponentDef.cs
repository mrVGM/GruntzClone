using Base.Actors;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gruntz.Projectile
{
    public class ProjectileComponentDef : ActorComponentDef
    {
        [Serializable]
        public class Parabola
        {
            public float Height = 2.0f;
            public int NumberOfPoints = 30;
            public float MinDist = 1.0f;
            public float MaxDist = 7.0f;
            public IEnumerable<Vector3> GetParabolaPoints(Vector3 startPoint, Vector3 endPoint)
            {
                float d = (endPoint - startPoint).magnitude;
                float c = -4.0f * Height / (d * d);

                Vector3 x = (endPoint - startPoint).normalized;
                Vector3 y = Vector3.up;

                for (int i = 0; i <= NumberOfPoints; ++i)
                {
                    float coef = (float)i / NumberOfPoints;
                    float cur = coef * d;
                    float h = c * (cur * cur - d * cur + d * d / 4.0f) + Height;

                    yield return startPoint + cur * x + h * y;
                }
            }
        }

        public Parabola ParabolaSettings;
        public float Speed = 3.0f;

        public override IActorComponent CreateActorComponent(Actor actor)
        {
            return new ProjectileComponent(actor, this);
        }
    }
}
