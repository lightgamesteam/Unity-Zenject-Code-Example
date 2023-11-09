using UnityEngine;

namespace XDPaint.Core.Materials
{
    public interface IMaterialsContainer
    {
        PaintMode PaintMode { get; set; }
        Texture SourceTexture { get; }
        int SourceTextureWidth { get; }
        int SourceTextureHeight { get; }
        Brush Brush { get; }
        Paint Paint { get; }
        bool Preview { get; set; }
    }
}