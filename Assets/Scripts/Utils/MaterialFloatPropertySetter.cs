using UnityEngine;

namespace Utils
{
    public class MaterialFloatPropertySetter : MonoBehaviour
    {
        public string PropertyName;
        public float PropertyValue;
        public Renderer Renderer => GetComponent<Renderer>();

        void Update()
        {
            var mat = Renderer.material;
            mat.SetFloat(PropertyName, PropertyValue);
        }
    }
}