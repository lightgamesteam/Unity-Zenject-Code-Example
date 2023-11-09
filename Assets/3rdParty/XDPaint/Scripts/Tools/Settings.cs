using UnityEngine;

namespace XDPaint.Tools
{
    [CreateAssetMenu(fileName = "XDPaintSettings", menuName = "XDPaint Settings", order = 100)]
    public class Settings : SingletonScriptableObject<Settings>
    {
        public Shader brushShader;
        public Shader paintShader;
        public Shader paintPreviewShader;
        public Shader eraseShader;
        public Shader erasePreviewShader;
        public Shader restoreShader;
        public Shader restorePreviewShader;
        public Texture defaultBrush;
        public bool undoRedoEnabled = true;
        public bool pressureEnabled = true;
        public uint brushDuplicatePartWidth = 4;
        
        [SerializeField]
        private uint _defaultTextureSize =  4096;
        public uint defaultTextureWidth => GetTextureSize();
        public uint defaultTextureHeight => GetTextureSize();

        public float pixelPerUnit = 100f;
        public string containerGameObjectName = "[XDPaintContainer]";

        private uint GetTextureSize()
        {
            //if(DeviceInfo.IsPC())
            return _defaultTextureSize;

            //return _defaultTextureSize / 2;
        }
    }
}