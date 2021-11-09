using System;
using UnityEngine;

namespace Utils
{
    public class CameraOverride : MonoBehaviour
    {
        [Serializable]
        public class Settings
        {
            public bool Orthographic;

            public void Extract(Camera cam)
            {
                Orthographic = cam.orthographic;
            }
            public void Apply(Camera cam)
            {
                cam.orthographic = Orthographic;
            }
        }

        public Settings OverrideSettings;
        private Settings OriginalSettings = new Settings();

        private void OnEnable()
        {
            var mainCamera = Camera.main;
            OriginalSettings.Extract(mainCamera);
            OverrideSettings.Apply(mainCamera);
        }

        private void OnDisable()
        {
            var mainCamera = Camera.main;
            OriginalSettings.Apply(mainCamera);
        }
    }
}