using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Model3D.View
{
    public class VideoView : MonoBehaviour
    {
        public CanvasGroup canvasGroup;

        [Header("Panel Video")]
        public GameObject panelVideo;
        public VideoPlayerBase videoPlayer;
        public Button closeButton;
        public Button saveButton;
        public Button sendButton;
        
        public class Factory : PlaceholderFactory<VideoView>
        {
        }
    }
}