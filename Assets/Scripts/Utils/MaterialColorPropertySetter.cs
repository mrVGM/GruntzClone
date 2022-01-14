using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    public class MaterialColorPropertySetter : MonoBehaviour
    {
        public string PropertyName;
        public Color PropertyValue;

        private Renderer _renderer;
        private Graphic _graphic;
        
        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            _graphic = GetComponent<Graphic>();
            if (_graphic != null) {
                var mat = _graphic.material;
                _graphic.material = Instantiate(mat);
            }
        }

        void Update()
        {
            if (_renderer != null) {
                var _propertyBlock = new MaterialPropertyBlock();
                _renderer.GetPropertyBlock(_propertyBlock);
                _propertyBlock.SetColor(PropertyName, PropertyValue);
                _renderer.SetPropertyBlock(_propertyBlock);
            }

            if (_graphic != null) {
                _graphic.material.SetColor(PropertyName, PropertyValue);
            }
        }
    }
}