using System;
using Gui.Core;
using TDL.Modules.Ultimate.Core.Managers;
using Zenject;

namespace TDL.Modules.Ultimate.GuiScrollbar {
    public interface IItemHandler {
        ItemScrollbarModelBase GetModel { get; }
        ItemScrollbarViewBase GetView { get; }
    }
    
    [Serializable]
    public abstract class ItemScrollbarControllerBase<TModel, TView> : MonoModelViewControllerBase<TModel, TView>, IItemHandler
        where TModel : ItemScrollbarModelBase
        where TView : ItemScrollbarViewBase {
        [Inject] private readonly ILanguageListeners _managerLanguageListeners = default;
        
        ItemScrollbarModelBase IItemHandler.GetModel => Model;
        ItemScrollbarViewBase IItemHandler.GetView => View;
        
        public override void Initialize() {
            base.Initialize();
            _managerLanguageListeners.LocalizeEvent += ChangeOfLanguage;
        }
        
        public virtual void Initialize(ItemScrollbarModelBase model) {
            Model.SetModel(model);
            RefreshView();
        }

        protected virtual void RefreshView() {
            View.DisplayText.text = Model.DisplayLabel;
            SetItemState(Model.ItemStateType);
            SetSelectState(Model.SelectStateType);
        }

        protected virtual void ChangeOfLanguage(ILanguageHandler languageHandler) {}
        
        private void SetItemState(ItemStateType itemState) {
            switch (itemState) {
                case ItemStateType.Disable:
                    View.ItemStateCanvasGroup.interactable = false;
                    break;
                case ItemStateType.Enable:
                    View.ItemStateCanvasGroup.interactable = true;
                    break;
                default:
                    View.ItemStateCanvasGroup.interactable = false;
                    break;
            }
        }
        
        private void SetSelectState(SelectStateType state) {
            DisableAll();
            switch (state) {
                case SelectStateType.None: {
                    View.SelectStateNoneCanvasGroup.SetActive(true);
                    break;
                }
                case SelectStateType.Active: {
                    View.SelectStateActiveCanvasGroup.SetActive(true);
                    break;
                }
                default: {
                    View.SelectStateNoneCanvasGroup.SetActive(true);
                    break;
                }
            }
        }

        private void DisableAll() {
            View.SelectStateNoneCanvasGroup.SetActive(false);
            View.SelectStateActiveCanvasGroup.SetActive(false);
        }
    }
}