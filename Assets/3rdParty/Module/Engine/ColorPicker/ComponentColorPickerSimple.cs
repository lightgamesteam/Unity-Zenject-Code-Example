using Module.Core.Component;

namespace Module.Engine.ColorPicker.Component {
    public class ComponentColorPickerSimple : ComponentVCBase<ComponentColorPickerSimpleView, ComponentColorPickerSimpleController> {
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
