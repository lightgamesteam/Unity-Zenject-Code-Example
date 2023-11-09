namespace Gui.Core {
    public abstract class GuiModelViewControllerBase<TModel, TView> : MonoModelViewControllerBase<TModel, TView>
        where TModel : ModelBase, new()
        where TView : ViewBase, new() {
        protected override bool UseAnchor => true;
    }
}