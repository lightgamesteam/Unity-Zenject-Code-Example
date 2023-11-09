using Module.Core.Component;

namespace Module.Engine.ControlElements.Component {
    public class ComponentControlElementsSimple : ComponentVCBase<ComponentControlElementsSimpleView, ComponentControlElementsSimpleController> {
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
