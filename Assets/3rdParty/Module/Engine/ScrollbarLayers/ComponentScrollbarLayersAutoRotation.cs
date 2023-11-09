using Module.Core.Component;

namespace Module.Engine.ScrollbarLayers.Component {
    public class ComponentScrollbarLayersAutoRotation : ComponentVCBase<ComponentScrollbarLayersAutoRotationView, ComponentScrollbarLayersAutoRotationController> {
        public override void ScreenOrientationToLandscape() {
            base.ScreenOrientationToLandscape();
            Controller.View.Portrait.HideComponent();
            Controller.View.Landscape.ShowComponent();
        }

        public override void ScreenOrientationToPortrait() {
            base.ScreenOrientationToPortrait();
            Controller.View.Landscape.HideComponent();
            Controller.View.Portrait.ShowComponent();
        }
    }
}
