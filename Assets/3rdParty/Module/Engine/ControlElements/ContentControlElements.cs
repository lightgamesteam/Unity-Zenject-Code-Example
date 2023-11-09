using Module.Core.Content;

namespace Module.Engine.ControlElements.Content {
    public class ContentControlElements : ContentVCBase<ContentControlElementsView, ContentControlElementsController> {
        public override void ShowComponent() {
            base.ShowComponent();
            View.MenuScene.ColorPickerToggle.IsOn = View.MenuScene.ColorPickerToggle.IsOn;
        }
    }
}
