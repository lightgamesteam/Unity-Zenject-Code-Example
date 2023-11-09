using Module.Core.UIComponent;
using TDL.Commands;
using TDL.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Module.Core {
    public class SelectableTextToSpeechController : ControllerBase {
        [Inject] protected readonly SignalBus Signal;

        protected GameObject CurrentSelectable;
        protected bool IsOnTextToAudio;

        protected override void Initialize() {
            base.Initialize();
            if (EventSystem.current.currentSelectedGameObject != null) {
                CurrentSelectable = EventSystem.current.currentSelectedGameObject;
            }

            IsOnTextToAudio = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio);
            if (IsOnTextToAudio) {
                SelectableLabelTextEvent.OnClick += FireTextToAudio;
            }

            InitializeButtons();
        }

        protected override void Release() {
            base.Release();
            if (IsOnTextToAudio) {
                if (SelectableLabelTextEvent.OnClick != null) {
                    SelectableLabelTextEvent.OnClick -= FireTextToAudio;
                }
            }
        }

        public void PlayTTS(string text) {
            if (!IsOnTextToAudio) { return; }

            FireTextToAudio(text);
        }

        private void Update() {
            if (!IsOnTextToAudio) { return; }
            if (EventSystem.current.currentSelectedGameObject == null) { return; }
            if (EventSystem.current.currentSelectedGameObject.HasComponent<ComponentButtonView>()) {
                CheckButton(EventSystem.current.currentSelectedGameObject.GetComponent<ComponentButtonView>());
            }
            if (CurrentSelectable == EventSystem.current.currentSelectedGameObject) { return; }

            CurrentSelectable = EventSystem.current.currentSelectedGameObject;
            _isMultiAudio = false;
            FireTextToAudio(GetText(CurrentSelectable));
        }

        private bool _isMultiAudio;

        private static string GetText(GameObject selectedGameObject) {
            var result = string.Empty;
            if (selectedGameObject.HasComponent<TextMeshProUGUI>()) {
                result = selectedGameObject.GetComponent<TextMeshProUGUI>().text;
            } else if (selectedGameObject.HasComponent<Text>()) {
                result = selectedGameObject.GetComponent<Text>().text;
            } else if (selectedGameObject.HasComponent<SelectableButtonTextEvent>()) {
                result = selectedGameObject.GetComponent<SelectableButtonTextEvent>().DisplayText;
            } else {
                GetChildTextMeshPro(ref result, selectedGameObject);
                GetChildText(ref result, selectedGameObject);
            }
            return result;
        }

        private static void GetChildText(ref string result, GameObject selectedGameObject) {
            var children = selectedGameObject.GetAllComponentsInChildren<Text>();
            children.RemoveAll(text => !text.isActiveAndEnabled);
            if (children.Count > 0) {
                result = children[0].text;
            }
        }

        private static void GetChildTextMeshPro(ref string result, GameObject selectedGameObject) {
            var children = selectedGameObject.GetAllComponentsInChildren<TextMeshProUGUI>();
            children.RemoveAll(text => !text.isActiveAndEnabled);
            if (children.Count > 0) {
                result = children[0].text;
            }
        }

        private void FireTextToAudio(string text) {
            if (string.IsNullOrEmpty(text)) { return; }

            Signal.Fire(new AccessibilityTTSPlayCommandSignal(text));
        }

        private void InitializeButtons() {
            var buttonViews = FindObjectsOfType<ComponentButtonView>();
            foreach (var componentButtonView in buttonViews) {
                CheckButton(componentButtonView);
            }
        }

        private void CheckButton(ComponentButtonView componentButtonView) {
            if (componentButtonView.IsMultiClick != IsOnTextToAudio) {
                componentButtonView.IsMultiClick = IsOnTextToAudio;
                componentButtonView.AddSingleClickListener(() => {
                    if (CurrentSelectable == EventSystem.current.currentSelectedGameObject) {
                        if (_isMultiAudio) {
                            FireTextToAudio(GetText(componentButtonView.gameObject));
                        } else {
                            _isMultiAudio = true;
                        }
                    }
                });
            }
        }
    }
}
