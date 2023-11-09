using UnityEngine;

namespace XDPaint.Core
{
	public interface IRenderTextureHelper
	{
		RenderTexture PaintTexture { get; }
		RenderTexture CombinedTexture { get; }
		
		void Init(int width, int height, RenderTexturesMode mode);
		void ReleaseTextures();
	}
}