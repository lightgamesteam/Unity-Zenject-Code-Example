using System;
using UnityEngine;
using XDPaint.Tools;

namespace XDPaint.Core.Materials
{
	[Serializable]
	public class Paint
	{
		private Material _material;
		public Material Material
		{
			get { return _material; }
		}

		private Color _color;
		private bool _initialized;

		private const string BrushOffsetShaderParam = "_BrushOffset";
		private const string BrushTextureShaderParam = "_BrushTex";
		private const string PaintTextureShaderParam = "_MaskTex";
		
		public void Init(Texture sourceTexture)
		{
			_material = new Material(Settings.Instance.paintShader);
			_material.mainTexture = sourceTexture;
			_material.color = _color;
			_initialized = true;
		}
		
		public void SetColor(Color color)
		{
			_color = color;
			if (_initialized)
			{
				_material.color = _color;			
			}
		}
        
		public void SetTexture(Texture texture)
		{
			if (_initialized)
			{
				_material.SetTexture(BrushTextureShaderParam, texture);
			}
		}
		
		public void SetPaintMode(PaintMode paintMode, bool preview, Texture brushTexture)
		{
			if (!_initialized)
				return;
			var shader = ShadersHelper.GetShader(paintMode, preview);
			_material.shader = shader;
			if (preview)
			{
				_material.SetTexture(BrushTextureShaderParam, brushTexture);
			}
		}

		public void SetPaintTexture(Texture texture)
		{
			if (!_initialized)
				return;
			_material.SetTexture(PaintTextureShaderParam, texture);
		}

		public void SetPaintPreviewVector(Vector4 brushOffset)
		{
			if (!_initialized)
				return;
			_material.SetVector(BrushOffsetShaderParam, brushOffset);
		}
	}
}