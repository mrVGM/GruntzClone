using Base.Actors;
using System.Collections.Generic;
using System.Linq;
using Base.UI;
using Gruntz.Equipment;

namespace Gruntz.UI
{
    public class UpdateEquipment : CoroutineProcess
    {
        public EquipmentSlot Weapon;
        public EquipmentSlot SpecialItem;
        public ProcessContextTagDef SelectedActorsTag;

        protected override IEnumerator<object> Crt()
        {
            while (true)
            {
                Weapon.gameObject.SetActive(false);
                SpecialItem.gameObject.SetActive(false);

                var selectedActors = context.GetItem(SelectedActorsTag) as IEnumerable<Actor>;
                if (selectedActors == null || selectedActors.Count() != 1)
                {
                    yield return null;
                    continue;
                }

                var actor = selectedActors.First();
                var equipment = actor.GetComponent<EquipmentComponent>();
                var weapon = equipment.Weapon;
                if (weapon != null) {
                    Weapon.EquipmentIcon.sprite = weapon.Icon;
                    Weapon.gameObject.SetActive(true);
                }

                var specialItem = equipment.SpecialItem;
                if (specialItem != null) {
                    SpecialItem.EquipmentIcon.sprite = specialItem.Icon;
                    SpecialItem.gameObject.SetActive(false);
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
