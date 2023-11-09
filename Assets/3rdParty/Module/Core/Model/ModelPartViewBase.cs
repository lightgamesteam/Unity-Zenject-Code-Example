using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Module.Core.Model {
    public abstract class ModelPartViewBase {
        public readonly string Name;
        public readonly Transform Transform;
        public readonly GameObject GameObject;
        public readonly Renderer[] RendererArray;
        public readonly MeshCollider[] MeshColliderArray;
        public readonly MeshFilter[] MeshFilterArray;
        public readonly Material[] DefaultMaterialArray;
        public readonly Shader[] DefaultMaterialShaderArray;
        public readonly Color[] DefaultMaterialColorArray;

        protected ModelPartViewBase(Transform transform) {
            Transform = transform;
            GameObject = Transform.gameObject;
            Name = Transform.name;

            RendererArray = ConnectRendererArray(GameObject);
            MeshColliderArray = ConnectMeshColliderArray(GameObject);
            MeshFilterArray = ConnectMeshFilterArray(GameObject);
            DefaultMaterialArray = ConnectDefaultMaterialArray(RendererArray);
            DefaultMaterialShaderArray = ConnectDefaultMaterialShaderArray(RendererArray);
            DefaultMaterialColorArray = ConnectDefaultMaterialColorArray(RendererArray);
            DisableReceiveShadows(RendererArray);
        }

        private static Renderer[] ConnectRendererArray(GameObject gameObject) {
            var result = new List<Renderer>();
            var rendererArray = gameObject.GetComponentsInChildren<Renderer>();
            if (rendererArray != null) {
                result.AddRange(rendererArray);
            }
            return result.ToArray();
        }

        private static MeshCollider[] ConnectMeshColliderArray(GameObject gameObject) {
            var result = new List<MeshCollider>();
            var meshColliderArray = gameObject.GetComponentsInChildren<MeshCollider>();
            if (meshColliderArray != null) {
                result.AddRange(meshColliderArray);
            }
            return result.ToArray();
        }

        private static MeshFilter[] ConnectMeshFilterArray(GameObject gameObject) {
            var result = new List<MeshFilter>();
            var meshFilterArray = gameObject.GetComponentsInChildren<MeshFilter>();
            if (meshFilterArray != null) {
                result.AddRange(meshFilterArray);
            }
            return result.ToArray();
        }

        private static Material[] ConnectDefaultMaterialArray(IEnumerable<Renderer> rendererArray) {
            return rendererArray.SelectMany(renderer => renderer.materials).ToArray();
        }

        private static Shader[] ConnectDefaultMaterialShaderArray(IEnumerable<Renderer> rendererArray) {
            return rendererArray.SelectMany(renderer => renderer.materials, (renderer, material) => material.shader).ToArray();
        }

        private static Color[] ConnectDefaultMaterialColorArray(IEnumerable<Renderer> rendererArray) {
            return (
                from renderer in rendererArray
                from material in renderer.materials
                where material.HasProperty("_Color")
                select material.color).ToArray();
        }

        private static void DisableReceiveShadows(IEnumerable<Renderer> rendererArray) {
            foreach (var renderer in rendererArray) {
                renderer.receiveShadows = false;
            }
        }
    }
}
