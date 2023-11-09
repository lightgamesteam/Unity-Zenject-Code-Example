using Module.Core.Component;

namespace Module.Engine.ScrollbarLayers.Component {
    public class ComponentScrollbarLayersSimple : ComponentVCBase<ComponentScrollbarLayersSimpleView, ComponentScrollbarLayersSimpleController> {
        public override void ScreenOrientationToLandscape() {
            base.ScreenOrientationToLandscape();
            Controller.View.Content.ShowComponent();
        }

        public override void ScreenOrientationToPortrait() {
            base.ScreenOrientationToPortrait();
            Controller.View.Content.ShowComponent();
        }
    }
}
