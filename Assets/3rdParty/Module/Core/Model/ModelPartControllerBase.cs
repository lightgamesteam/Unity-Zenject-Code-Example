namespace Module.Core.Model {
    public abstract class ModelPartControllerBase : PartControllerBase {
        public readonly ModelPartViewBase View;

        public override void RefreshVisible() {
            base.RefreshVisible();
            if (!View.GameObject.activeSelf.Equals(IsActive)) {
                View.GameObject.SetActive(IsActive);
            }
        }
        
        protected ModelPartControllerBase(ModelPartViewBase view) {
            View = view;
            RefreshVisible();
        }
    }
}