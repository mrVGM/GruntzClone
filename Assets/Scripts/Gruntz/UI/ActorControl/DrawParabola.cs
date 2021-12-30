using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.Equipment;
using Gruntz.Projectile;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class DrawParabola : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;
        public ProcessContextTagDef ParabolaActive;
        public LineRenderer Parabola;

        protected override IEnumerator<object> Crt()
        {
            ProjectileAttackAbilityDef ability = context.GetItem(ParabolaActive) as ProjectileAttackAbilityDef;

            if (ability == null) {
                yield break;
            }

            var selected = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            var actor = selected.First();

            ProjectileAttackAbilityDef getProjectileAbility()
            {
                var equipment = actor.GetComponent<EquipmentComponent>();
                if (equipment.Weapon == null)
                {
                    return null;
                }

                return equipment.Weapon.Abilities.OfType<ProjectileAttackAbilityDef>().FirstOrDefault();
            }

            void drawParabola()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));
                if (floorHit.collider == null)
                {
                    return;
                }

                Vector3 offset = floorHit.point - actor.Pos;
                var x = offset.normalized;
                float d = offset.magnitude;

                var projectile = ability.Projectile;
                var parabola = projectile.Components.OfType<ProjectileComponentDef>().FirstOrDefault().ParabolaSettings;

                d = Mathf.Clamp(d, parabola.MinDist, parabola.MaxDist);

                var points = parabola.GetParabolaPoints(actor.Pos, actor.Pos + d * x);
                var positions = points.ToArray();
                Parabola.positionCount = positions.Length;
                Parabola.SetPositions(positions);
            }

            while (true) {
                if (ability != getProjectileAbility()) {
                    yield break;
                }
                drawParabola();
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            Parabola.positionCount = 2;
            Parabola.SetPositions(new Vector3[] { 1000 * Vector3.down, 1000 * Vector3.down + Vector3.right });
            yield break;
        }
    }
}
