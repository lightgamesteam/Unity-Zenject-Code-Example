using Module.Core.Component;
using Module.Core.UIContent;

namespace Module.Engine.ControlElements.Component {
    public class ComponentControlElementsAutoRotationController : ComponentControllerBase<ComponentControlElementsAutoRotationView> {
        public ContentButton MenuApplicationCloseButton;
        public ContentToggle MenuApplicationFullscreenToggle;
        public ContentButtonTrigger MenuSceneZoomPlusButton;
        public ContentButtonTrigger MenuSceneZoomMinusButton;
        public ContentToggle MenuSceneColorPickerToggle;
        public ContentButton MenuSceneResetButton;
        public ContentToggle MenuModuleLayersToggle;
        public ContentText InfoModuleText;

        protected override void Initialize() {
            base.Initialize();
            MenuApplicationCloseButton = new ContentButton(
                View.Landscape.View.MenuApplication.CloseButton, 
                View.Portrait.View.MenuApplication.CloseButton);
            MenuApplicationFullscreenToggle = new ContentToggle(
                View.Landscape.View.MenuApplication.FullscreenToggle, 
                View.Portrait.View.MenuApplication.FullscreenToggle);
            MenuSceneZoomPlusButton = new ContentButtonTrigger(
                View.Landscape.View.MenuScene.ZoomPlusButton, 
                View.Portrait.View.MenuScene.ZoomPlusButton);
            MenuSceneZoomMinusButton = new ContentButtonTrigger(
                View.Landscape.View.MenuScene.ZoomMinusButton, 
                View.Portrait.View.MenuScene.ZoomMinusButton);
            MenuSceneColorPickerToggle = new ContentToggle(
                View.Landscape.View.MenuScene.ColorPickerToggle, 
                View.Portrait.View.MenuScene.ColorPickerToggle);
            MenuSceneResetButton = new ContentButton(
                View.Landscape.View.MenuScene.ResetButton, 
                View.Portrait.View.MenuScene.ResetButton);
            MenuModuleLayersToggle = new ContentToggle(
                View.Landscape.View.MenuModule.LayersToggle, 
                View.Portrait.View.MenuModule.LayersToggle);
            InfoModuleText = new ContentText(
                View.Landscape.View.InfoModule.Text, 
                View.Portrait.View.InfoModule.Text);

#if UI_ANDROID || UI_IOS
            MenuSceneZoomPlusButton.SetActive(false);
            MenuSceneZoomMinusButton.SetActive(false);
#endif
        }
    }
}
