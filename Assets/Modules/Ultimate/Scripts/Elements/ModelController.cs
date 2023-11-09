using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Module.Core.Model;
using TDL.Modules.Ultimate.Core.ActivityData;
using UnityEngine;

namespace TDL.Modules.Ultimate.Core.Elements {
    public class ModelController : ModelPartControllerBase {
        public readonly DataBase Data;
        
        private const string SHADER_NAME = "Highlight/XRay";
        private static readonly int HighlightColor = Shader.PropertyToID("_HighlightColor");
        private static readonly int XRayColor = Shader.PropertyToID("_XRAyColor");
        private static readonly int ZTestXRay = Shader.PropertyToID("_ZTestXRay");
        
        private readonly List<Material> _highlightMaterials = new List<Material>();
        private const float HIGHLIGHT_SPEED = 0.5f;
        private Color _colorStart;
        private Color _colorEnd;
        private Color _xRayColor;

        public ModelController(Transform transform, DataBase data) : base(new ElementView(transform)) {
            Data = data;
            if (data is ActivityData.LabelData label) {
                CreateHighlightMaterial(label.HighlightColor);
            }
        }
        
        public void SetHighlightColor(bool isHighlighted) {
            foreach (var highlightMaterial in _highlightMaterials) {
                if (isHighlighted) {
                    highlightMaterial.DOColor(_colorEnd, HighlightColor, HIGHLIGHT_SPEED).SetEase(Ease.OutExpo);
                } else {
                    highlightMaterial.DOColor(_colorStart, HighlightColor, HIGHLIGHT_SPEED).SetEase(Ease.OutExpo);
                }
            }
        }

        private void CreateHighlightMaterial(Color color) {
            _xRayColor = color;
            _colorStart = _colorEnd = color;
            _colorStart.a = 0f;
            _colorEnd.a = 0.8f;
            
            foreach (var renderer in View.RendererArray) {
                _highlightMaterials.Add(AddMaterial(_colorStart, renderer));
            }
        }
        
        private Material AddMaterial(Color color, Renderer renderer) {
            var result = new Material(Shader.Find(SHADER_NAME));
            result.SetColor(HighlightColor, color);
            result.SetColor(XRayColor, _xRayColor);
            var materials = renderer.sharedMaterials.ToList();
            materials.Add(result);
            renderer.materials = materials.ToArray();
            return result;
        }
    }
}