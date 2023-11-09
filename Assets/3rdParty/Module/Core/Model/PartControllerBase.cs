namespace Module.Core.Model {
    public abstract class PartControllerBase {
        public bool IsActive { get; private set; }

        public void SwitchActiveAndRefresh() {
            SetActiveAndRefresh(!IsActive);
        }

        public void SetActiveAndRefresh(bool isActive) {
            IsActive = isActive;
            RefreshVisible();
        }

        public virtual void RefreshVisible() {}
        
        protected PartControllerBase() {
            IsActive = true;
        }
    }
}