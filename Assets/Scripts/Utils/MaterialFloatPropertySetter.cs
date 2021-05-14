using System;
using UnityEngine;

namespace Utils
{
    public class MaterialFloatPropertySetter : MonoBehaviour
    {
        [Serializable]
        public class Property
        {
            public string PropertyName;
            public AnimationCurve Curve;
        }
        public float Progress;
        public Property[] Properties;

        public Renderer Renderer => GetComponent<Renderer>();

        void UpdateFor(float progress)
        {
            var mat = Renderer.material;
            foreach (var prop in Properties)
            {
                mat.SetFloat(prop.PropertyName, prop.Curve.Evaluate(progress));
            }
        }

        private void Update()
        {
            UpdateFor(Progress);
        }

        private void Awake()
        {
            UpdateFor(0);
        }
    }
}