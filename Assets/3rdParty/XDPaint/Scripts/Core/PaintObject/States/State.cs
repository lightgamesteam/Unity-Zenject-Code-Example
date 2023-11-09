using System.Collections.Generic;
using UnityEngine;

namespace XDPaint.Core.PaintObject.States
{
    public class State
    {
        public List<DrawingState> DrawingStates = new List<DrawingState>();
        public readonly PaintMode PaintMode;
        public readonly Texture BrushTexture;
        public readonly float BrushSize;
        public readonly Color BrushColor;

        public State(PaintMode paintMode, Texture brushTexture, Color brushColor, float brushSize)
        {
            PaintMode = paintMode;
            BrushTexture = brushTexture;
            BrushColor = brushColor;
            BrushSize = brushSize;
        }
    }
}