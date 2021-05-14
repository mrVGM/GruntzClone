using UnityEngine;

namespace Utils
{
    public class MaterialColorPropertySetter : MonoBehaviour
    {
        public string PropertyName;
        public Color PropertyValue;
        public Renderer Renderer => GetComponent<Renderer>();

        void Update()
        {
            var mat = Renderer.material;
            mat.SetColor(PropertyName, PropertyValue);
        }
    }
}