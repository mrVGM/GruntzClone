using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class FPSCounter : MonoBehaviour
    {
        public Text Text;
        private int _worst = 0;
        private int _currentWorst = int.MaxValue;
        public float RefreshTime = 1;
        public float LastTimeRefreshed = float.NegativeInfinity;
        private void Update()
        {
            if (Time.unscaledTime - LastTimeRefreshed > RefreshTime)
            {
                _worst = _currentWorst;
                _currentWorst = int.MaxValue;
                LastTimeRefreshed = Time.unscaledTime;
            }

            int fps = Mathf.FloorToInt(1 / Time.unscaledDeltaTime);
            if (fps < _currentWorst) {
                _currentWorst = fps;
            }

            Text.text = $"{_worst}";
        }
    }
}