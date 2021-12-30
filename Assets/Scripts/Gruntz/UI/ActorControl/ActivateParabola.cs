using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.Equipment;
using Gruntz.Projectile;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class ActivateParabola : CoroutineProcess
    {
        public ProcessContextTagDef SelectedActorsTag;
        public ProcessContextTagDef ParabolaActive;
        public Button WeaponButton;

        protected override IEnumerator<object> Crt()
        {
            context.PutItem(ParabolaActive, null);
            IEnumerable<Actor> selectedActors;
            while (true) {
                selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
                if (selectedActors != null && selectedActors.Count() == 1) {
                    break;
                }
                yield return null;
            }
            var actor = selectedActors.FirstOrDefault();

            ProjectileAttackAbilityDef getProjectileAbility()
            {
                var equipment = actor.GetComponent<EquipmentComponent>();
                if (equipment.Weapon == null) {
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
            context.PutItem(ParabolaActive, ability);
        }

        protected override IEnumerator<object> FinishCrt()
        {
            WeaponButton.onClick.RemoveAllListeners();
            yield break;
        }
    }
}
