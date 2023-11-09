namespace Gui.Core {
    public abstract class GuiModelControllerBase<TModel> : MonoModelControllerBase<TModel> 
        where TModel : ModelBase, new() {
        protected override bool UseAnchor => true;
    }
}