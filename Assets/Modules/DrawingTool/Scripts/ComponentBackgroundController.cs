using Module.Core.Component;
using UnityEngine;

namespace DrawingTool.Components.ControlElements {
    public class ComponentBackgroundController : ComponentControllerBase<ComponentBackgroundView> {
        public void SetBackground(Texture2D texture2D) {
            if (texture2D == null) {
                return;
            }
            
            if (View.BackgroundImage.sprite != null) {
                var oldSprite = View.BackgroundImage.sprite;
                View.BackgroundImage.sprite = null;
                Object.Destroy(oldSprite);
            }
            View.BackgroundImage.sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(.5f, .5f));
        }
    }
}