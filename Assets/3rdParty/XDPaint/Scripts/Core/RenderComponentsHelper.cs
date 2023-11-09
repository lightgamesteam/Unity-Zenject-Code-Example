using UnityEngine;
using UnityEngine.UI;
using XDPaint.Tools;

namespace XDPaint.Core
{
    [System.Serializable]
    public class RenderComponentsHelper : IRenderComponentsHelper
    {
        public ObjectComponentType ComponentType { get; private set; }

        public Component PaintComponent { get; private set; }
        public Component RendererComponent { get; private set; }

        public Material Material
        {
            get
            {
                if (ComponentType == ObjectComponentType.CanvasImage)
                {
                    return ((RawImage) RendererComponent).material;
                }
                return ((Renderer) RendererComponent).sharedMaterial;
            }
            set
            {
                var rawImage = PaintComponent as RawImage;
                if (rawImage != null)
                {
                    rawImage.material = value;
                    return;
                }

                var rendererComponent = PaintComponent as Renderer;
                if (rendererComponent != null)
                {
                    rendererComponent.sharedMaterial = value;
                }
            }
        }

        public void Init(GameObject gameObject, out ObjectComponentType componentType)
        {
            var canvasImage = gameObject.GetComponent<RawImage>();
            if (canvasImage != null)
            {
                PaintComponent = canvasImage;
                RendererComponent = PaintComponent;
                ComponentType = componentType = ObjectComponentType.CanvasImage;
                return;
            }

            var meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                PaintComponent = meshFilter;
                RendererComponent = gameObject.GetComponent<MeshRenderer>();
                if (RendererComponent == null)
                {
                    Debug.LogError("Can't find MeshRenderer component!");
                }
                ComponentType = componentType = ObjectComponentType.MeshFilter;
                return;
            }

            var skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
            if (skinnedMeshRenderer != null)
            {
                PaintComponent = skinnedMeshRenderer;
                RendererComponent = PaintComponent;
                ComponentType = componentType = ObjectComponentType.SkinnedMeshRenderer;
                return;
            }

            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                PaintComponent = spriteRenderer;
                RendererComponent = PaintComponent;
                ComponentType = componentType = ObjectComponentType.SpriteRenderer;
                return;
            }

            Debug.LogError("Can't find render component in ObjectForPainting field!");
            ComponentType = componentType = ObjectComponentType.Unknown;
        }

        public bool IsMesh()
        {
            return RendererComponent is MeshRenderer || RendererComponent is SkinnedMeshRenderer;
        }

        public Texture GetSourceTexture(Material material, string shaderTextureName)
        {
            var width = (int)Settings.Instance.defaultTextureWidth;
            var height = (int)Settings.Instance.defaultTextureHeight;
            if (ComponentType == ObjectComponentType.SkinnedMeshRenderer || ComponentType == ObjectComponentType.MeshFilter)
            {
                if (!string.IsNullOrEmpty(shaderTextureName))
                {
                    var texture = material.GetTexture(shaderTextureName);
                    if (texture == null)
                    {
                        texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                    }
                    return texture;
                }
            }
            else if (ComponentType == ObjectComponentType.SpriteRenderer)
            {
                var spriteRenderer = RendererComponent as SpriteRenderer;
                if (spriteRenderer != null)
                {
                    if (spriteRenderer.sprite == null)
                    {
                        var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
                        var pixelPerUnit = Settings.Instance.pixelPerUnit;
                        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one / 2f, pixelPerUnit);
                    }
                    return spriteRenderer.sprite.texture;
                }
            }
            else if (ComponentType == ObjectComponentType.CanvasImage)
            {
                var image = RendererComponent as RawImage;
                if (image != null)
                {
                    if (image.texture == null)
                    {
                        var rect = image.rectTransform.rect;
                        image.texture = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.ARGB32, false);
                    }
                    return image.texture;
                }
            }
            return null;
        }

        public void SetSourceMaterial(Material material)
        {
            if (ComponentType == ObjectComponentType.SkinnedMeshRenderer || ComponentType == ObjectComponentType.MeshFilter || ComponentType == ObjectComponentType.SpriteRenderer)
            {
                var renderer = RendererComponent as Renderer;
                if (renderer != null)
                {
                    renderer.material = material;
                    var spriteRenderer = renderer as SpriteRenderer;
                    if (spriteRenderer != null)
                    {
                        var materialPropertyBlock = new MaterialPropertyBlock();
                        materialPropertyBlock.SetTexture("_MainTex", material.mainTexture);
                        spriteRenderer.SetPropertyBlock(materialPropertyBlock);
                    }   
                }
            }
            else if (ComponentType == ObjectComponentType.CanvasImage)
            {
                var image = RendererComponent as RawImage;
                if (image != null)
                {
                    image.material = material;
                    image.texture = material.mainTexture;
                }
            }
        }

        public Mesh GetMesh()
        {
            if (IsMesh())
            {
                var meshFilter = PaintComponent as MeshFilter;
                if (meshFilter != null)
                {
                    return meshFilter.sharedMesh;
                }
                
                var skinnedMeshRenderer = PaintComponent as SkinnedMeshRenderer;
                if (skinnedMeshRenderer != null)
                {
                    return skinnedMeshRenderer.sharedMesh;
                }
            }
            Debug.LogError("Can't find MeshFilter or SkinnedMeshRenderer component!");
            return null;
        }
    }
}