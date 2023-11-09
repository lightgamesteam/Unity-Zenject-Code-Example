using System.Collections.Generic;
using Module.Core.Patterns;
using UnityEngine;

namespace Module.ManagerMaterials {
    public class ManagerMaterials : MonoBehaviourSingleton<ManagerMaterials> {
        #region Properties

        [SerializeField] protected MaterialVariants Standard;
        [SerializeField] protected MaterialVariants StandardSpecular;
        [SerializeField] protected MaterialVariants Autodesk;

        #endregion

        private void Start() {
            DontDestroyOnLoad(transform.root.gameObject);
        }

        private enum RenderingMode {
            NONE = -1,
            OPAQUE = 0,
            CUTOUT = 1,
            FADE = 2,
            TRANSPARENT = 3
        }

        private const string SHADER_STANDARD = "Standard";
        private const string SHADER_STANDARD_SPECULAR = "Standard (Specular setup)";
        private const string SHADER_STANDARD_ROUGHNESS = "Standard (Roughness setup)";
        private const string SHADER_AUTODESK_INTERACTIVE = "Autodesk Interactive";

        public void ReplaceMaterials(GameObject gameObject) {
            var meshRendererArray = gameObject.GetComponentsInChildren<Renderer>();
            if (meshRendererArray == null || meshRendererArray.Length == 0) {
                return;
            }
            foreach (var meshRenderer in meshRendererArray) {
                ReplaceMaterial(meshRenderer);
            }
        }

        public void ReplaceMaterial(Renderer renderer) {
            var materials = new List<Material>();
            foreach (var m in renderer.materials) {
                var shaderName = (m.shader.name ?? "").ToLower();
                if (shaderName.Equals(SHADER_STANDARD_SPECULAR.ToLower())) {
                    materials.Add(GetStandardSpecularMaterial(m, GetRenderingMode(m)));
                } else if (shaderName.Equals(SHADER_STANDARD_ROUGHNESS.ToLower())
                           || shaderName.Equals(SHADER_AUTODESK_INTERACTIVE.ToLower())) {
                    materials.Add(GetStandardRoughnessMaterial(m, GetRenderingMode(m)));
                } else if (shaderName.Equals(SHADER_STANDARD.ToLower())) {
                    materials.Add(GetStandardMaterial(m, GetRenderingMode(m)));
                } else if (shaderName.Contains("legacy")) {
                    materials.Add(shaderName.Contains("specular") 
                        // ? ReplaceToStandardSpecularMaterial(m, shaderName) 
                        ? ReplaceToStandardMaterial(m, shaderName) 
                        : ReplaceToStandardMaterial(m, shaderName));
                } else {
                    var material = new Material(m);
                    material.CopyPropertiesFromMaterial(m);
                    materials.Add(material);
                }
            }
            renderer.materials = materials.ToArray();
            renderer.sharedMaterials = materials.ToArray();
        }

        private Material ReplaceToStandardSpecularMaterial(Material current, string shaderName) {
            return shaderName.Contains("cutout")
                ? GetStandardSpecularMaterial(current, RenderingMode.CUTOUT)
                : (shaderName.Contains("fade")
                    ? GetStandardSpecularMaterial(current, RenderingMode.FADE)
                    : GetStandardSpecularMaterial(current,
                        shaderName.Contains("transpa") ? RenderingMode.TRANSPARENT : RenderingMode.OPAQUE));
        }

        private Material ReplaceToStandardMaterial(Material current, string shaderName) {
            return shaderName.Contains("cutout")
                ? GetStandardMaterial(current, RenderingMode.CUTOUT)
                : (shaderName.Contains("fade")
                    ? GetStandardMaterial(current, RenderingMode.FADE)
                    : GetStandardMaterial(current,
                        shaderName.Contains("transpa") ? RenderingMode.TRANSPARENT : RenderingMode.OPAQUE));
        }

        private Material GetStandardMaterial(Material current, RenderingMode mode = RenderingMode.NONE) {
            return GetMaterial(current, Standard, mode);
        }

        private Material GetStandardSpecularMaterial(Material current, RenderingMode mode = RenderingMode.NONE) {
            return GetMaterial(current, StandardSpecular, mode);
        }

        private Material GetStandardRoughnessMaterial(Material current, RenderingMode mode = RenderingMode.NONE) {
            return GetMaterial(current, Autodesk, mode);
        }

        private static Material GetMaterial(Material current, MaterialVariants variants, RenderingMode mode) {
            Material result;
            switch (mode) {
                case RenderingMode.CUTOUT:
                    result = new Material(variants.Cutout);
                    break;
                case RenderingMode.FADE:
                    result = new Material(variants.Fade);
                    break;
                case RenderingMode.TRANSPARENT:
                    result = new Material(variants.Transparent);
                    break;
                default:
                    result = new Material(variants.Opaque);
                    break;
            }
            CopyPropertiesFromMaterial(current, result);
            return result;
        }

        private static RenderingMode GetRenderingMode(Material current) {
            return current.HasProperty("_Mode") ? (RenderingMode)Mathf.RoundToInt(current.GetFloat("_Mode")) : RenderingMode.NONE;
        }

        private static void CopyColorProperty(Material fromMaterial, Material toMaterial, string propertyName) {
            if (fromMaterial.HasProperty(propertyName)) {
                toMaterial.SetColor(propertyName, fromMaterial.GetColor(propertyName));
            }
        }

        private static void CopyFloatProperty(Material fromMaterial, Material toMaterial, string propertyName) {
            if (fromMaterial.HasProperty(propertyName)) {
                toMaterial.SetFloat(propertyName, fromMaterial.GetFloat(propertyName));
            }
        }

        private static void CopyIntProperty(Material fromMaterial, Material toMaterial, string propertyName) {
            if (fromMaterial.HasProperty(propertyName)) {
                toMaterial.SetInt(propertyName, fromMaterial.GetInt(propertyName));
            }
        }

        private static void CopyRangeProperty(Material fromMaterial, Material toMaterial, string propertyName) {
            if (fromMaterial.HasProperty(propertyName)) {
                toMaterial.SetFloat(propertyName, fromMaterial.GetFloat(propertyName));
            }
        }

        private static void CopyTextureProperty(Material fromMaterial, Material toMaterial, string propertyName) {
            if (fromMaterial.HasProperty(propertyName)) {
                toMaterial.SetTexture(propertyName, fromMaterial.GetTexture(propertyName));
            }
        }

        private static void IsKeywordEnabled(Material fromMaterial, Material toMaterial, string propertyName, bool isForce = false) {
            if (fromMaterial.IsKeywordEnabled(propertyName) || isForce) {
                toMaterial.EnableKeyword(propertyName);
                AddShaderKeywords(toMaterial, propertyName);
            } else {
                toMaterial.DisableKeyword(propertyName);
            }
        }

        private static void AddShaderKeywords(Material toMaterial, params string[] items) {
            // Validate the parameters
            var target = toMaterial.shaderKeywords ?? new string[] { };
            if (items == null) {
                items = new string[] { };
            }
            // Join the arrays
            var result = new string[target.Length + items.Length];
            target.CopyTo(result, 0);
            items.CopyTo(result, target.Length);
            toMaterial.shaderKeywords = result;
        }

        private static void CopyPropertiesFromMaterial(Material fromMaterial, Material toMaterial) {
            IsKeywordEnabled(fromMaterial, toMaterial, "_NORMALMAP");
            IsKeywordEnabled(fromMaterial, toMaterial, "_EMISSION");
            IsKeywordEnabled(fromMaterial, toMaterial, "_PARALLAXMAP");
            IsKeywordEnabled(fromMaterial, toMaterial, "_DETAIL_MULX2");
            if (fromMaterial.HasProperty("_Metallic") || fromMaterial.HasProperty("_MetallicGlossMap")) {
                IsKeywordEnabled(fromMaterial, toMaterial, "_METALLICGLOSSMAP");
            }
            IsKeywordEnabled(fromMaterial, toMaterial, "_SPECGLOSSMAP");

            CopyTextureProperty(fromMaterial, toMaterial, "_MainTex");
            CopyColorProperty(fromMaterial, toMaterial, "_Color");
            CopyRangeProperty(fromMaterial, toMaterial, "_Cutoff");
            CopyFloatProperty(fromMaterial, toMaterial, "_SmoothnessTextureChannel");
            CopyRangeProperty(fromMaterial, toMaterial, "_Glossiness");
            CopyRangeProperty(fromMaterial, toMaterial, "_GlossMapScale");
            CopyTextureProperty(fromMaterial, toMaterial, "_MetallicGlossMap");
            CopyRangeProperty(fromMaterial, toMaterial, "_Metallic");
            CopyTextureProperty(fromMaterial, toMaterial, "_SpecGlossMap");
            CopyColorProperty(fromMaterial, toMaterial, "_SpecColor");
            CopyFloatProperty(fromMaterial, toMaterial, "_SpecularHighlights");
            CopyFloatProperty(fromMaterial, toMaterial, "_GlossyReflections");
            CopyTextureProperty(fromMaterial, toMaterial, "_BumpMap");
            CopyFloatProperty(fromMaterial, toMaterial, "_BumpScale");
            CopyTextureProperty(fromMaterial, toMaterial, "_ParallaxMap");
            CopyRangeProperty(fromMaterial, toMaterial, "_Parallax");
            CopyTextureProperty(fromMaterial, toMaterial, "_OcclusionMap");
            CopyRangeProperty(fromMaterial, toMaterial, "_OcclusionStrength");
            CopyTextureProperty(fromMaterial, toMaterial, "_EmissionMap");
            CopyColorProperty(fromMaterial, toMaterial, "_EmissionColor");
            CopyTextureProperty(fromMaterial, toMaterial, "_DetailMask");
            CopyTextureProperty(fromMaterial, toMaterial, "_DetailAlbedoMap");
            CopyTextureProperty(fromMaterial, toMaterial, "_DetailNormalMap");
            CopyFloatProperty(fromMaterial, toMaterial, "_DetailNormalMapScale");
            CopyFloatProperty(fromMaterial, toMaterial, "_UVSec");

            //CopyFloatProperty(fromMaterial, toMaterial, "_SrcBlend");
            //CopyFloatProperty(fromMaterial, toMaterial, "_DstBlend");
            //CopyFloatProperty(fromMaterial, toMaterial, "_ZWrite");
        }
    }
}
