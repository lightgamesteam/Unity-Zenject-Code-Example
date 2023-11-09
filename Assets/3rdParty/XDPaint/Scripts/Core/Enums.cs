namespace XDPaint.Core
{
	public enum ObjectComponentType
	{
		Unknown,
		CanvasImage,
		MeshFilter,
		SkinnedMeshRenderer,
		SpriteRenderer
	}
	
	public enum PaintMode
	{
		Paint = 0,
		Erase = 2,
		EraseBackground = 4,
		Restore = 6
	}
	
	public enum RenderTexturesMode
	{
		OneTexture = 0,
		TwoTextures = 1
	}
}