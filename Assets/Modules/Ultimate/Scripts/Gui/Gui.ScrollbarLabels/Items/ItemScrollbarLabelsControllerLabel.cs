using TDL.Modules.Ultimate.GuiScrollbar;

namespace TDL.Modules.Ultimate.GuiScrollbarLabels {
    public class ItemScrollbarLabelsControllerLabel : ItemScrollbarLabelsControllerBase<ItemScrollbarLabelsModelLabel, ItemScrollbarLabelsViewLabel> {
        protected override void ChangeOfLabel(int id, bool elementActive) {
            if (!Model.Id.Equals(id)) { return; }

            if (Model.EyeStateType != EyeStateType.None) {
                Model.EyeStateType = !elementActive ? EyeStateType.Shown : EyeStateType.Hidden;
                RefreshView();
            } else {
                base.ChangeOfLabel(id, elementActive);
            }
        }
        
        protected override void ChangeOfLabel(int id, bool layerActive, bool labelActive) {
            base.ChangeOfLabel(id, layerActive, labelActive);
            if (!Model.Id.Equals(id)) { return; }

            if (layerActive) {
                Model.SelectStateType = labelActive ? SelectStateType.Active : SelectStateType.None;
                Model.EyeStateType = EyeStateType.None;
            } else {
                Model.SelectStateType = SelectStateType.None;
                Model.EyeStateType = !labelActive ? EyeStateType.Shown : EyeStateType.Hidden;
            }
            RefreshView();
        }

        protected override void RefreshView() {
            base.RefreshView();
            SetEyeState(Model.EyeStateType);
        }

        private void SetEyeState(EyeStateType state) {
            DisableAll();
            switch (state) {
                case EyeStateType.None: {
                    View.EyeStateNoneCanvasGroup.SetActive(true);
                    break;
                }
                case EyeStateType.Shown: {
                    View.EyeStateShowCanvasGroup.SetActive(true);
                    break;
                }
                case EyeStateType.Hidden: {
                    View.EyeStateHideCanvasGroup.SetActive(true);
                    break;
                }
                default: {
                    View.EyeStateNoneCanvasGroup.SetActive(true);
                    break;
                }
            }
        }
        
        private void DisableAll() {
            View.EyeStateNoneCanvasGroup.SetActive(false);
            View.EyeStateShowCanvasGroup.SetActive(false);
            View.EyeStateHideCanvasGroup.SetActive(false);
        }
    }
}