using UnityEngine;
using UnityEngine.Rendering;
using XDPaint.Core.Materials;
using XDPaint.Tools.Raycast;

namespace XDPaint.Core.PaintObject.Base
{
	public class BasePaintObjectRenderer
	{
		public bool UseNeighborsVertices
		{
			set { _lineDrawer.UseNeighborsVertices = value; }
		}
		
		protected Camera Camera
		{
			set { _lineDrawer.Camera = value; }
		}

		protected IMaterialsContainer MaterialsContainer;
		private BaseLineDrawer _lineDrawer;
		private Mesh _mesh;
		private Mesh _quadMesh;
		private CommandBuffer _commandBuffer;
		private RenderTargetIdentifier _rti;
		private RenderTargetIdentifier _rtiCopy;
		private RenderTargetIdentifier _rtiCombined;
		private readonly Vector3 _upRight = new Vector3(1, 1, 0);

		protected void InitRenderer(Camera camera, IRenderTextureHelper renderTextureHelper, IMaterialsContainer materialsContainer)
		{
			_mesh = new Mesh();
			MaterialsContainer = materialsContainer;
			_lineDrawer = new BaseLineDrawer();
			var sourceTextureSize = new Vector2(materialsContainer.SourceTextureWidth, materialsContainer.SourceTextureHeight);
			_lineDrawer.Init(camera, sourceTextureSize, RenderLine);

			_commandBuffer = new CommandBuffer {name = "XDPaintObject"};
			_rti = new RenderTargetIdentifier(renderTextureHelper.PaintTexture);
			_rtiCopy = new RenderTargetIdentifier(renderTextureHelper.PaintTexture);
			_rtiCombined = new RenderTargetIdentifier(renderTextureHelper.CombinedTexture);

			InitQuadMesh();
		}	
		
		private void InitQuadMesh()
		{
			_quadMesh = new Mesh();
			_quadMesh.vertices = new Vector3[4];
			_quadMesh.uv = new[]
			{
				Vector2.up, 
				Vector2.one, 
				Vector2.right, 
				Vector2.zero, 
			};
			_quadMesh.triangles = new[]
			{
				0, 1, 2,
				2, 3, 0
			};
			_quadMesh.colors = new[]
			{
				Color.white,
				Color.white,
				Color.white,
				Color.white
			};
		}
		
		protected void ClearRenderTexture()
		{
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_rti);
			_commandBuffer.ClearRenderTarget(false, true, Color.clear);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
		}

		protected void ClearCombined()
		{
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_rtiCombined);
			_commandBuffer.ClearRenderTarget(false, true, Color.clear);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
		}

		protected void UpdateQuad(Rect positionRect, bool execute = true)
		{
			_quadMesh.vertices = new[]
			{
				new Vector3(positionRect.xMin, positionRect.yMax, 0),
				new Vector3(positionRect.xMax, positionRect.yMax, 0),
				new Vector3(positionRect.xMax, positionRect.yMin, 0),
				new Vector3(positionRect.xMin, positionRect.yMin, 0)
			};
			GL.LoadOrtho();
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_rti);
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, MaterialsContainer.Brush.Material);
			if (execute)
			{
				Graphics.ExecuteCommandBuffer(_commandBuffer);
			}
		}

		protected void SetDefaultQuad()
		{
			_quadMesh.vertices = new[]
			{
				Vector3.up,
				_upRight,
				Vector3.right,
				Vector3.zero
			};
		}

		protected Vector2[] DrawLine(Vector2 fistPaintPos, Vector2 lastPaintPos, Triangle firstTriangle, Triangle lastTriangle)
		{
			return _lineDrawer.DrawLine(fistPaintPos, lastPaintPos, firstTriangle, lastTriangle);
		}

		protected void PreDrawCombined()
		{
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_rtiCopy);
		}

		protected void DrawCombined(int pass)
		{
			GL.LoadOrtho();
			_commandBuffer.DrawMesh(_quadMesh, Matrix4x4.identity, MaterialsContainer.Paint.Material, 0, pass);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
		}

		protected void RenderLine(Vector2[] drawLine, Texture brushTexture, float brushSizeActual, float[] brushSizes)
		{
			_lineDrawer.RenderLine(drawLine, brushTexture, brushSizeActual, brushSizes);
		}

		private void RenderLine(Vector3[] positions, Vector2[] uv, int[] indices, Color[] colors)
		{
			if (_mesh != null)
			{
				_mesh.Clear(false);
			}

			_mesh.vertices = positions;
			_mesh.uv = uv;
			_mesh.triangles = indices;
			_mesh.colors = colors;
			_commandBuffer.Clear();
			_commandBuffer.SetRenderTarget(_rti);
			GL.LoadOrtho();
			_commandBuffer.DrawMesh(_mesh, Matrix4x4.identity, MaterialsContainer.Brush.Material);
			Graphics.ExecuteCommandBuffer(_commandBuffer);
		}
	}
}