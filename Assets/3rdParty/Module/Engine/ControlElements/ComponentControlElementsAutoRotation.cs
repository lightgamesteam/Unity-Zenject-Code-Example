using Module.Core.Component;

namespace Module.Engine.ControlElements.Component {
    public class ComponentControlElementsAutoRotation : ComponentVCBase<ComponentControlElementsAutoRotationView, ComponentControlElementsAutoRotationController> {
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
