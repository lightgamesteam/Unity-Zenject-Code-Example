namespace Gui.Core {
    public abstract class GuiViewControllerBase<TView> : MonoViewControllerBase<TView>
        where TView : ViewBase, new() {
        protected override bool UseAnchor => true;
    }
}