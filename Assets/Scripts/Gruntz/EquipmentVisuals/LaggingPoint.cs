using Base;
using UnityEngine;

namespace Gruntz.EquipmentVisuals
{
    public class LaggingPoint : MonoBehaviour
    {
        public TagDef Tag;
        public LaggingPointTarget Target;

        private void Update()
        {
            transform.position = Target.GetPosition(this);
        }
    }
}
