using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.Equipment;
using Gruntz.Items;
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
                d = Mathf.Clamp(d, ability.ParabolaSettings.MinDist, ability.ParabolaSettings.MaxDist);

                var points = ability.ParabolaSettings.GetParabolaPoints(actor.Pos, actor.Pos + d * x);
                var positions = points.ToArray();
                Parabola.positionCount = positions.Length;
                Parabola.SetPositions(positions);
            }

            do {
                drawParabola();
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
