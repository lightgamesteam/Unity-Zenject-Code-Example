using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Model3D.View
{
    public class ScreenshotView : MonoBehaviour
    {
        public Image shotImage;
        public Button shotCloseButton;
        public Button shotSaveButton;
        public Button shotSendButton;
        public CanvasGroup canvasGroup;
        
        public class Factory : PlaceholderFactory<ScreenshotView>
        {
        }
    }
}