using Module.Core;
using Module.Core.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace Module.Engine.ScrollbarLayers.Item {
    public class ItemScrollbarLayersPartView : ItemScrollbarLayersPartViewBase {
        [LabelOverride("Eye.State -> None")]
        [SerializeField] protected CanvasGroup EyeStateNoneCanvasGroup;
        [LabelOverride("Eye.State -> Shown")]
        [SerializeField] protected CanvasGroup EyeStateShowCanvasGroup;
        [LabelOverride("Eye.State -> Hidden")]
        [SerializeField] protected CanvasGroup EyeStateHideCanvasGroup;

        public override void Initialize(string displayText, UnityAction action) {
            base.Initialize(displayText, action);
            SetEyeState(EyeStateType.None);
        }

        public virtual void SetEyeState(EyeStateType state) {
            switch (state) {
                case EyeStateType.None: {
                    Utilities.Component.SetActiveCanvasGroup(EyeStateNoneCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateShowCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateHideCanvasGroup, false);
                    break;
                }
                case EyeStateType.Shown: {
                    Utilities.Component.SetActiveCanvasGroup(EyeStateNoneCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateShowCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateHideCanvasGroup, false);
                    break;
                }
                case EyeStateType.Hidden: {
                    Utilities.Component.SetActiveCanvasGroup(EyeStateNoneCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateShowCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateHideCanvasGroup, true);
                    break;
                }
                default: {
                    Utilities.Component.SetActiveCanvasGroup(EyeStateNoneCanvasGroup, true);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateShowCanvasGroup, false);
                    Utilities.Component.SetActiveCanvasGroup(EyeStateHideCanvasGroup, false);
                    break;
                }
            }
        }
    }
}
