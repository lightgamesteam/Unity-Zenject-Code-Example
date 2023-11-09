using UnityEngine;

namespace XDPaint.Core
{
	public class RenderTextureHelper : IRenderTextureHelper
	{
		public RenderTexture PaintTexture { get; private set; }
		public RenderTexture CombinedTexture { get; private set; }

		public void Init(int width, int height, RenderTexturesMode mode)
		{
			PaintTexture = CreateRenderTexture(width, height);
			if (mode == RenderTexturesMode.TwoTextures)
			{
				CombinedTexture = CreateRenderTexture(width, height);
			}
		}

		private RenderTexture CreateRenderTexture(int width, int height)
		{
			var renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32)
			{
				filterMode = FilterMode.Bilinear,
				autoGenerateMips = false
			};
			renderTexture.Create();
			
			return renderTexture;
		}

		public void ReleaseTextures()
		{
			if (PaintTexture != null && PaintTexture.IsCreated())
			{
				PaintTexture.Release();
			}
			if (CombinedTexture != null && CombinedTexture.IsCreated())
			{
				CombinedTexture.Release();
			}
		}
	}
}