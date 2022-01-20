using System.Collections.Generic;
using System.Linq;
using Base.Actors;
using Base.MessagesSystem;
using Base.UI;
using Gruntz.Abilities;
using Gruntz.Equipment;
using Gruntz.Projectile;
using Gruntz.UnitController;
using Gruntz.UnitController.Instructions;
using UnityEngine;
using Utils;

namespace Gruntz.UI.ActorControl
{
    public class ShootWithParabola : CoroutineProcess
    {
        public ProcessContextTagDef HitResultsTag;
        public ProcessContextTagDef SelectedActorsTag;
        public ProcessContextTagDef ParabolaActive;
        public MessagesBoxTagDef MessagesBox;

        private enum AbilityPresence
        {
            AbilityPrent,
            AbilityMissing,
            Finished
        }

        private enum MainCrtState
        {
            WaitingForAbilityEnabled,
            WaitingForInput,
            Finished,
        }

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

            IEnumerator<AbilityPresence> abilityPresent()
            {
                var main = mainCrt();
                while (true)
                {
                    var projectileAbility = getProjectileAbility();
                    if (ability != projectileAbility) {
                        yield return AbilityPresence.AbilityMissing;
                        break;
                    }
                    main.MoveNext();
                    if (main.Current == MainCrtState.Finished) {
                        yield return AbilityPresence.Finished;
                        break;
                    }
                    yield return AbilityPresence.AbilityPrent;
                }
            }

            IEnumerator<MainCrtState> mainCrt()
            {
                while (Input.GetAxis("Ability") > 0) {
                    yield return MainCrtState.WaitingForInput;
                }
                
                var abilitiesComponent = actor.GetComponent<AbilitiesComponent>();
                while (!abilitiesComponent.IsEnabled(ability)) {
                    yield return MainCrtState.WaitingForAbilityEnabled;
                }

                while (Input.GetAxis("Ability") <= 0) {
                    yield return MainCrtState.WaitingForInput;
                }

                while (Input.GetAxis("Ability") > 0) {
                    yield return MainCrtState.WaitingForInput;
                }

                var hits = context.GetItem(HitResultsTag) as IEnumerable<RaycastHit>;
                var floorHit = hits.FirstOrDefault(x => x.collider.gameObject.layer == LayerMask.NameToLayer(UnityLayers.Floor));
                
                Vector3 offset = floorHit.point - actor.Pos;
                var x = offset.normalized;
                float d = offset.magnitude;
                var projectile = ability.Projectile;
                var parabola = projectile.Components.OfType<ProjectileComponentDef>().FirstOrDefault().ParabolaSettings;
                d = Mathf.Clamp(d, parabola.MinDist, parabola.MaxDist);

                var messagesSystem = MessagesSystem.GetMessagesSystemFromContext();
                messagesSystem.SendMessage(
                    MessagesBox,
                    Base.MainUpdaterUpdateTime.Update,
                    this,
                    new UnitControllerInstruction {
                        Unit = actor,
                        Executable = new StopAndShootProjectile(actor.Pos + d * x, ability),
                    });

                yield return MainCrtState.Finished;
            }

            var crt = abilityPresent();

            while (true) {
                crt.MoveNext();
                if (crt.Current == AbilityPresence.AbilityMissing || crt.Current == AbilityPresence.Finished) {
                    break;
                }
                yield return null;
            }
        }

        protected override IEnumerator<object> FinishCrt()
        {
            yield break;
        }
    }
}
