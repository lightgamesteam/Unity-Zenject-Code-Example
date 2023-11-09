using Module.Core.Component;

namespace Module.Engine.ScrollbarLanguages.Component {
    public class ComponentScrollbarLanguagesSimple : ComponentVCBase<ComponentScrollbarLanguagesSimpleView, ComponentScrollbarLanguagesSimpleController> {
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
