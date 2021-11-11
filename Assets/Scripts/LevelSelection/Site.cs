using Base;
using UnityEngine;

namespace LevelSelection
{
    public class Site : MonoBehaviour
    {
        public enum TooltipPos
        {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft
        }

        public Transform TooltipWorldPosition;
        public TooltipPos TooltipRelativePos;

        public void HighlightActive(bool active)
        {
            var animator = GetComponent<Animator>();
            animator.SetInteger("State", active ? 1 : 0);
        }
    }
}
