using Module.Core.Component;
using Module.Core.UIContent;

namespace Module.Engine.ControlElements.Component {
    public class ComponentControlElementsSimpleController : ComponentControllerBase<ComponentControlElementsSimpleView> {
        public ContentButton MenuApplicationCloseButton;
        public ContentToggle MenuApplicationFullscreenToggle;
        public ContentButtonTrigger MenuSceneZoomPlusButton;
        public ContentButtonTrigger MenuSceneZoomMinusButton;
        public ContentToggle MenuSceneColorPickerToggle;
        public ContentButton MenuSceneResetButton;
        public ContentToggle MenuModuleLanguagesToggle;
        public ContentToggle MenuModuleLabelsToggle;
        public ContentToggle MenuModuleLayersToggle;
        public ContentText InfoModuleText;

        protected override void Initialize() {
            base.Initialize();
            MenuApplicationCloseButton = new ContentButton(View.Content.View.MenuApplication.CloseButton);
            MenuApplicationFullscreenToggle = new ContentToggle(View.Content.View.MenuApplication.FullscreenToggle);
            MenuSceneZoomPlusButton = new ContentButtonTrigger(View.Content.View.MenuScene.ZoomPlusButton);
            MenuSceneZoomMinusButton = new ContentButtonTrigger(View.Content.View.MenuScene.ZoomMinusButton);
            MenuSceneColorPickerToggle = new ContentToggle(View.Content.View.MenuScene.ColorPickerToggle);
            MenuSceneResetButton = new ContentButton(View.Content.View.MenuScene.ResetButton);
            MenuModuleLanguagesToggle = new ContentToggle(View.Content.View.MenuModule.LanguagesToggle);
            MenuModuleLabelsToggle = new ContentToggle(View.Content.View.MenuModule.LabelsToggle);
            MenuModuleLayersToggle = new ContentToggle(View.Content.View.MenuModule.LayersToggle);
            InfoModuleText = new ContentText(View.Content.View.InfoModule.Text);

#if UI_ANDROID || UI_IOS
            MenuSceneZoomPlusButton.SetActive(false);
            MenuSceneZoomMinusButton.SetActive(false);
#endif
        }
    }
}
