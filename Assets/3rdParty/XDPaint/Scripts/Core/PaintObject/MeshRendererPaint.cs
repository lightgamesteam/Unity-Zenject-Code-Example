using UnityEngine;
using XDPaint.Core.PaintObject.Base;

namespace XDPaint.Core.PaintObject
{
	public sealed class MeshRendererPaint : BasePaintObject
	{
		private Renderer _renderer;

		protected override void Init()
		{
			_renderer = ObjectTransform.GetComponent<Renderer>();

			Mesh mesh = null;
			var meshFilter = ObjectTransform.GetComponent<MeshFilter>();
			if (meshFilter != null)
			{
				mesh = meshFilter.sharedMesh;
			}
			else
			{
				var skinnedMeshRenderer = ObjectTransform.GetComponent<SkinnedMeshRenderer>();
				if (skinnedMeshRenderer != null)
				{
					mesh = skinnedMeshRenderer.sharedMesh;
				}
			}
			if (mesh == null)
			{
				Debug.LogError("Can't find MeshFilter or SkinnedMeshRenderer component!");
			}
			if (Camera.orthographic)
			{
				Debug.LogWarning("Camera is not perspective!");
			}
		}

		private bool IsInBounds(Vector3 position)
		{
			var bounds = new Bounds();
			if (_renderer != null)
			{
				bounds = _renderer.bounds;
			}
			var ray = Camera.ScreenPointToRay(position);
			var inBounds = bounds.IntersectRay(ray);
			return inBounds;
		}

		protected override void OnPaint(Vector3 position, Vector2? uv = null)
		{
			if (!IsInBounds(position))
			{
				PaintPosition = null;
				OnPostPaint();
				return;
			}

			bool hasRaycast = uv != null;
			if (hasRaycast)
			{
				PaintPosition = new Vector2(uv.Value.x * MaterialsContainer.SourceTextureWidth, uv.Value.y * MaterialsContainer.SourceTextureHeight);
			}
			else
			{
				PaintPosition = null;
			}
			OnPostPaint();
		}
	}
}