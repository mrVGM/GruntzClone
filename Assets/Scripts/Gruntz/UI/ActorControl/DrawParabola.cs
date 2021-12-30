using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.Equipment;
using Gruntz.Items;
using Gruntz.Projectile;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class DrawParabola : CoroutineProcess
    {
        private enum CrtState
        {
            Running,
            EquipmentChanged,
        }

        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;
        public float Height;
        public int NumberOfPoints = 30;
        public LineRenderer Parabola;
        public Button WeaponButton;

        protected override IEnumerator<object> Crt()
        {
            var selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
            if (selectedActors == null || selectedActors.Count() != 1) {
                yield break;
            }
            var actor = selectedActors.FirstOrDefault();

            ProjectileAttackAbilityDef getProjectileAbility()
            {
                var equipment = actor.GetComponent<EquipmentComponent>();
                if (equipment.Weapon == null)
                {
                    return null;
                }

                return equipment.Weapon.Abilities.OfType<ProjectileAttackAbilityDef>().FirstOrDefault();
            }

            ProjectileAttackAbilityDef ability = null;
            WeaponButton.onClick.AddListener(() => {
                ability = getProjectileAbility();
            });

            while (ability == null) {
                yield return null;
            }

            Vector3 tmp = Vector3.zero;
            void drawParabola()
            {
                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));
                if (floorHit.collider == null) {
                    return;
                }

                Vector3 offset = floorHit.point - actor.Pos;
                var x = offset.normalized;
                float d = offset.magnitude;

                var projectile = ability.Projectile;
                var parabola = projectile.Components.OfType<ProjectileComponentDef>().FirstOrDefault().ParabolaSettings;

                d = Mathf.Clamp(d, parabola.MinDist, parabola.MaxDist);

                tmp = actor.Pos + d * x;
                var points = parabola.GetParabolaPoints(actor.Pos, tmp);
                var positions = points.ToArray();
                Parabola.positionCount = positions.Length;
                Parabola.SetPositions(positions);
            }

            bool spawned = false;
            do {
                drawParabola();
                if (!spawned && Input.GetAxis("Ability") > .1f) {
                    spawned = true;
                    var projectile = Actors.ActorDeployment.DeployActorFromTemplate(ability.Projectile, -1000 * Vector3.down);
                    var projectileComponent = projectile.GetComponent<ProjectileComponent>();
                    var projectileData = projectileComponent.Data as ProjectileComponentData;
                    projectileData.StartPoint = actor.Pos;
                    projectileData.EndPoint = tmp;
                    projectileComponent.Data = projectileData;
                }
                yield return null;
            } while (ability == getProjectileAbility());
        }

        protected override IEnumerator<object> FinishCrt()
        {
            WeaponButton.onClick.RemoveAllListeners();
            yield break;
        }
    }
}
