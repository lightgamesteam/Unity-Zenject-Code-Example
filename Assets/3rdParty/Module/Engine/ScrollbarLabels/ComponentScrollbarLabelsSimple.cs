using Module.Core.Component;

namespace Module.Engine.ScrollbarLabels.Component {
    public class ComponentScrollbarLabelsSimple : ComponentVCBase<ComponentScrollbarLabelsSimpleView, ComponentScrollbarLabelsSimpleController> {
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
