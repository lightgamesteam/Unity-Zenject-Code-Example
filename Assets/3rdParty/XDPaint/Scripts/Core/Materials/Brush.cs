using System;
using UnityEngine;
using XDPaint.Tools;

namespace XDPaint.Core.Materials
{
	[Serializable]
	public class Brush
	{
		[SerializeField] private Material _material;
		public Material Material
		{
			get { return _material; }
		}
		
		[SerializeField] private Color color = Color.white;
		public Color Color
		{
			get { return color; }
			set
			{
				color = value;
				if (_initialized)
				{
					SetBrushColorAlpha(color);
					if (onChangeColor != null)
					{
						onChangeColor(color);
					}
				}
			}
		}
                
		[SerializeField] private Texture texture;
		public Texture Texture
		{
			get { return texture; }
			set
			{
				texture = value;
				if (_initialized)
				{
					SetTexture(texture);
				}
			}
		}
        
		[SerializeField] private float size = 1f;
		public float Size
		{
			get { return size; }
			set
			{
				size = value;
			}
		}

		public delegate void OnChangeColor(Color color);
		public delegate void OnChangeTexture(Texture texture);
		public OnChangeColor onChangeColor;
		public OnChangeTexture onChangeTexture;
		
		private bool _initialized;
		private const string ColorAlphaShaderParam = "_ColorAlpha";
		private const string MainTextureShaderParam = "_MainTex";
		private const string SrcBlend = "_SrcBlend";
		private const string DstBlend = "_DstBlend";

		public void Init()
		{
			_material = new Material(Settings.Instance.brushShader);
			_material.mainTexture = texture;
			SetBrushColorAlpha(color);
			if (onChangeColor != null)
			{
				onChangeColor(color);
			}
			if (onChangeTexture != null)
			{
				onChangeTexture(texture);
			}
			_initialized = true;
		}

		public void SetPaintMode(PaintMode paintMode)
		{
			if (!_initialized)
				return;
			if (paintMode == PaintMode.Paint)
			{
				_material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
				_material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			}
			else if (paintMode == PaintMode.Erase)
			{
				_material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.Zero);
				_material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			}
			else if (paintMode == PaintMode.EraseBackground)
			{
				_material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
				_material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			}
			else if (paintMode == PaintMode.Restore)
			{
				_material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
				_material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			}
		}
		
		private void SetBrushColorAlpha(Color color)
		{
			_material.color = color;
			_material.SetFloat(ColorAlphaShaderParam, color.a);
		}

		private void SetTexture(Texture texture)
		{
			_material.SetTexture(MainTextureShaderParam, texture);
			if (onChangeTexture != null)
			{
				onChangeTexture(texture);
			}
		}
	}
}