using System.Collections.Generic;
using UnityEngine;

namespace Module.Core {
    public partial class Utilities {
        public class Material {
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

            public static void ReplaceMaterials(GameObject gameObject) {
                var meshRenderers = gameObject.GetComponentsInChildren<Renderer>();
                if (meshRenderers == null || meshRenderers.Length == 0) {
                    return;
                }
                foreach (var meshRenderer in meshRenderers) {
                    ReplaceMaterial(meshRenderer);
                }
            }

            public static void ReplaceMaterial(Renderer renderer) {
                var materials = new List<UnityEngine.Material>();
                foreach (var m in renderer.materials) {
                    var shaderName = (m.shader.name ?? "").ToLower();
                    if (shaderName.Equals(SHADER_STANDARD_SPECULAR.ToLower())) {
                        materials.Add(GetStandardSpecularMaterial(m, true));
                    } else if (shaderName.Equals(SHADER_STANDARD_ROUGHNESS.ToLower()) 
                               || shaderName.Equals(SHADER_AUTODESK_INTERACTIVE.ToLower())) {
                        materials.Add(GetStandardRoughnessMaterial(m, true));
                    } else if (shaderName.Equals(SHADER_STANDARD.ToLower())) {
                        materials.Add(GetStandardMaterial(m, true));
                    } else if (shaderName.Contains("specular")) {
                        materials.Add(ReplaceToStandardSpecularMaterial(m, shaderName));
                    } else {
                        materials.Add(ReplaceToStandardMaterial(m, shaderName));
                    }
                }
                renderer.materials = materials.ToArray();
            }

            private static UnityEngine.Material ReplaceToStandardSpecularMaterial(UnityEngine.Material current, string shaderName) {
                return shaderName.Contains("cutout")
                    ? GetStandardSpecularMaterial(current, mode:RenderingMode.CUTOUT)
                    : (shaderName.Contains("fade")
                        ? GetStandardSpecularMaterial(current, mode:RenderingMode.FADE)
                        : GetStandardSpecularMaterial(current,
                            mode:shaderName.Contains("transpa") ? RenderingMode.TRANSPARENT : RenderingMode.OPAQUE));
            }

            private static UnityEngine.Material ReplaceToStandardMaterial(UnityEngine.Material current, string shaderName) {
                return shaderName.Contains("cutout")
                    ? GetStandardMaterial(current, mode:RenderingMode.CUTOUT)
                    : (shaderName.Contains("fade")
                        ? GetStandardMaterial(current, mode:RenderingMode.FADE)
                        : GetStandardMaterial(current,
                            mode:shaderName.Contains("transpa") ? RenderingMode.TRANSPARENT : RenderingMode.OPAQUE));
            }

            private static UnityEngine.Material GetStandardMaterial(UnityEngine.Material current, bool isCopyProperties = false, RenderingMode mode = RenderingMode.NONE) {
                var shader = Shader.Find(SHADER_STANDARD);
                return GetMaterial(current, shader, isCopyProperties, mode);
            }

            private static UnityEngine.Material GetStandardSpecularMaterial(UnityEngine.Material current, bool isCopyProperties = false, RenderingMode mode = RenderingMode.NONE) {
                var shader = Shader.Find(SHADER_STANDARD_SPECULAR);
                return GetMaterial(current, shader, isCopyProperties, mode);
            }

            private static UnityEngine.Material GetStandardRoughnessMaterial(UnityEngine.Material current, bool isCopyProperties = false, RenderingMode mode = RenderingMode.NONE) {
                var shader = Shader.Find(SHADER_AUTODESK_INTERACTIVE);
                return GetMaterial(current, shader, isCopyProperties, mode);
            }

            private static UnityEngine.Material GetMaterial(UnityEngine.Material current, Shader shader, bool isCopyProperties, RenderingMode mode) {
                var result = new UnityEngine.Material(current) { shader = shader };
                if (isCopyProperties) {
                    result.CopyPropertiesFromMaterial(current);
                }
                //ChangeRenderMode(result, mode);
                return result;
            }

            private static void ChangeRenderMode(UnityEngine.Material material, RenderingMode mode) {
                switch (mode) {
                    case RenderingMode.NONE:
                        break;
                    case RenderingMode.OPAQUE:
                        material.SetFloat("_Mode", 0);
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = -1;
                        break;
                    case RenderingMode.CUTOUT:
                        material.SetFloat("_Mode", 1);
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.EnableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 2450;
                        break;
                    case RenderingMode.FADE:
                        material.SetFloat("_Mode", 2);
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.EnableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                    case RenderingMode.TRANSPARENT:
                        material.SetFloat("_Mode", 3);
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;
                        break;
                }
            }
        }
    }
}
