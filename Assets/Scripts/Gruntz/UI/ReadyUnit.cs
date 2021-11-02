using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gruntz.UI
{
    public class ReadyUnit : MonoBehaviour, IPointerDownHandler
    {
        public Action PointerDown;
        public Image Filling;

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown?.Invoke();
        }
    }
}
