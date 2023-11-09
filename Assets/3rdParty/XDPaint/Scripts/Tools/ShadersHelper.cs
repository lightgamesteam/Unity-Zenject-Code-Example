using System.Collections.Generic;
using UnityEngine;
using XDPaint.Core;

namespace XDPaint.Tools
{
	public static class ShadersHelper
	{
		private static readonly Dictionary<int, Shader> PaintModeShaders = new Dictionary<int, Shader>()
		{
			{ ((int)PaintMode.Paint) + 1, Settings.Instance.paintPreviewShader },
			{ ((int)PaintMode.Paint) + 0, Settings.Instance.paintShader },
			{ ((int)PaintMode.Erase) + 1, Settings.Instance.paintPreviewShader },
			{ ((int)PaintMode.Erase) + 0, Settings.Instance.paintShader },
			{ ((int)PaintMode.EraseBackground) + 1, Settings.Instance.erasePreviewShader },
			{ ((int)PaintMode.EraseBackground) + 0, Settings.Instance.eraseShader },
			{ ((int)PaintMode.Restore) + 1, Settings.Instance.restorePreviewShader },
			{ ((int)PaintMode.Restore) + 0, Settings.Instance.restoreShader }
		};

		public static Shader GetShader(PaintMode paintMode, bool preview)
		{
			var key = ((int)paintMode) + (preview ? 1 : 0);
			return PaintModeShaders[key];
		}
	}
}