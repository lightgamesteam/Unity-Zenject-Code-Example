using System;
using System.Collections.Generic;
using System.Linq;
using Module.Core;
using TDL.Commands;
using TDL.Constants;
using TDL.Models;
using TDL.Modules.Ultimate.Core;
using TDL.Modules.Ultimate.GuiScrollbar;
using TDL.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace TDL.Views
{
    public class SelectableTextToSpeechMediator : IInitializable, IDisposable, ITickable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private LocalizationModel _localizationModel;
        [Inject] private AsyncProcessorService _asyncProcessor;
        [Inject] private ContentViewModel _contentViewModel;

        private readonly List<string> workInScene = new List<string>
        {
            SceneNameConstants.Main,
            SceneNameConstants.Module3DModel,
            SceneNameConstants.ModuleQuiz,
            SceneNameConstants.ModulePuzzle,
            SceneNameConstants.Module2DVideo,
            SceneNameConstants.Module3DVideo,
            SceneNameConstants.ModuleConcaveAndConvex,
            SceneNameConstants.ModuleClassification,
            SceneNameConstants.ModulePeriodicTable,
            SceneNameConstants.ModuleDrawingTool,
            SceneNameConstants.ModuleUltimate
        };

        private List<string> loadedScenes = new List<string>();
        private bool _isWork => workInScene.Contains(currentScene);
        
        private string currentScene
        {
            get
            {
                if(loadedScenes.Count > 0)
                    return loadedScenes.Last();

                return SceneManager.GetActiveScene().name;
            }
        }
        
        private GameObject _previousSelectable;
        private bool _isOnTTS;
        
        public void PlayTTS(string text)
        {
            if (_isOnTTS && _isWork && !string.IsNullOrEmpty(text))
            {
                TTS(text);
            }
        }

        public void PlayTTS(string text, GameObject go)
        {
            if (_isOnTTS && _isWork && !string.IsNullOrEmpty(text))
            {
                TTS(text, GetCurrentLanguage(go));
            }
        }

        #region Init

        public void Initialize()
        {
            SubscribeOnListeners();

            if (EventSystem.current.currentSelectedGameObject != null)
                _previousSelectable = EventSystem.current.currentSelectedGameObject;

            _isOnTTS = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityTextToAudio);
        }

        private void SubscribeOnListeners()
        {
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
            
            ClickTextEvent.onClick += PlayTTS;
            SelectableLabelTextEvent.OnClick += PlayTTS;
            ObjectHighlighter.ttsAction += PlayTTS;

            _signal.Subscribe<AccessibilityTextToAudioClickViewSignal>(UpdateIsOnTTS);
            _signal.Subscribe<AccessibilityTTSPlayOnHoverViewSignal>(OnHoverGameObject);
        }

        private void SceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
        {
            if (!loadedScenes.Contains(_scene.name))
                loadedScenes.Add(_scene.name);
        }

        private void SceneUnloaded(Scene _scene)
        {
            if (loadedScenes.Contains(_scene.name))
                loadedScenes.Remove(_scene.name);
        }
        
        private void OnHoverGameObject(AccessibilityTTSPlayOnHoverViewSignal signal)
        {
            ProcessTTS(signal.ObjectWithAudio);
        }

        private void UpdateIsOnTTS(AccessibilityTextToAudioClickViewSignal signal)
        {
            _isOnTTS = signal.IsEnabled;
        }

        public void Dispose()
        {
            SceneManager.sceneLoaded -= SceneLoaded;
            SceneManager.sceneUnloaded -= SceneUnloaded;
            
            ClickTextEvent.onClick -= PlayTTS;
            SelectableLabelTextEvent.OnClick -= PlayTTS;
            ObjectHighlighter.ttsAction -= PlayTTS;

            _signal.Unsubscribe<AccessibilityTextToAudioClickViewSignal>(UpdateIsOnTTS);
            _signal.Unsubscribe<AccessibilityTTSPlayOnHoverViewSignal>(OnHoverGameObject);
        }
        
        #endregion

        #region Inputs

        public void Tick()
        {
            if (_isOnTTS && _isWork)
            {
                if (UserInput())
                {
                    Interact();
                }
            }
        }

        private void Interact()
        {
            if (EventSystem.current.currentSelectedGameObject == null)
                return;

            _asyncProcessor.Wait(0, () =>
            {
                var selectedObject = EventSystem.current.currentSelectedGameObject;

                if (selectedObject)
                {
                    ProcessTTS(selectedObject);
                }
            });
        }

        private bool UserInput()
        {
            if (Input.touchCount == 0)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    return true;
                }
            }
            else
            {
                if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended)
                {
                    return true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) ||
                Input.GetKeyDown(KeyCode.Space))
            {
                return true;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    return true;
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion
        
        private void ProcessTTS(GameObject gameObject)
        {
            if (_previousSelectable != gameObject)
            {
                _previousSelectable = gameObject;
                FindText(_previousSelectable);
            }
        }

        private void FindText(GameObject currentSelectable)
        {
            var txt = string.Empty;
            var lang = string.Empty;
            
            if (currentSelectable.HasComponent(out TextToSpeechDontSayAnythingState skipState))
            {
                return;
            }

            if (currentSelectable.HasComponent(out Text text))
            {
                txt = text.text;
                lang = GetCurrentLanguage(currentSelectable);
            }
            else if (currentSelectable.HasComponent(out TextMeshProUGUI textMesh))
            {
                txt = textMesh.text;
                lang = GetCurrentLanguage(currentSelectable);
            }
            else if (currentSelectable.HasComponent(out TooltipEvents tooltip))
            {
                txt = tooltip.GetHint();
                lang = GetCurrentLanguage(tooltip.gameObject);
            }
            else if (currentSelectable.HasComponent(out TextToSpeechItemWithNoText withNoText))
            {
                txt = withNoText.GetTranslation();
                lang = GetCurrentLanguage(currentSelectable);
            }
            else if (currentSelectable.HasComponent(out SelectableButtonTextEvent selectableEvent))
            {
                txt = selectableEvent.DisplayText;
                lang = GetCurrentLanguage(currentSelectable);
            }
            else
            {
                GetChildText(ref txt, ref lang, currentSelectable);
                GetChildTextMeshPro(ref txt, ref lang, currentSelectable);
            }

            if (!string.IsNullOrEmpty(txt))
            {
                if (currentSelectable.HasComponent(out Toggle tgl)
                    && !currentSelectable.HasComponent(out TextToSpeechDontSayToggleState deafToggle))
                {
                    if (currentSelectable.HasComponent(out TooltipEvents tooltip) && tooltip.GetSpeechInvertedResult())
                    {
                        txt += GetToggleStatus(!tgl.isOn, lang);
                    }
                    else
                    {
                        txt += GetToggleStatus(tgl.isOn, lang);
                    }
                }

                TTS(txt, lang);
            } else if (currentSelectable.HasComponent(out TextToSpeechOnHoverScrollbar scrollbar)) {
                txt = scrollbar.Handler.GetModel.DisplayLabel;
                lang = GetCurrentLanguage(currentSelectable);
                if (currentSelectable.HasComponent(out TooltipEvents tooltip) && tooltip.GetSpeechInvertedResult()) {
                    txt += GetToggleStatus(scrollbar.Handler.GetModel.SelectStateType != SelectStateType.Active , lang);
                } else {
                    txt += GetToggleStatus(scrollbar.Handler.GetModel.SelectStateType == SelectStateType.Active, lang);
                }
                if (!string.IsNullOrEmpty(txt)) {
                    TTS(txt, lang);
                }
            }
        }

        private void GetChildText(ref string result, ref string language, GameObject selectedGameObject)
        {
            var children = selectedGameObject.GetAllComponentsInChildren<Text>();
            children.RemoveAll(text => !text.isActiveAndEnabled);
            if (children.Count > 0)
            {
                result = children[0].text;
                language = GetCurrentLanguage(children[0].gameObject);
            }
        }

        private void GetChildTextMeshPro(ref string result, ref string language, GameObject selectedGameObject)
        {
            var children = selectedGameObject.GetAllComponentsInChildren<TextMeshProUGUI>();
            children.RemoveAll(text => !text.isActiveAndEnabled);
            if (children.Count > 0)
            {
                result = children[0].text;
                language = GetCurrentLanguage(children[0].gameObject);
            }
        }

        private string GetToggleStatus(bool status, string language)
        {
            return status
                ? _localizationModel.AllSystemTranslations[language][LocalizationConstants.AccessibilityIsEnabledKey]
                : _localizationModel.AllSystemTranslations[language][LocalizationConstants.AccessibilityIsDisabledKey];
        }

        private void TTS(string text, string cc = "")
        {
            _signal.Fire(new AccessibilityTTSPlayCommandSignal(text, language: cc));
        }

        private string GetCurrentLanguage(GameObject go)
        {
            switch (go.scene.name)
            {
                case SceneNameConstants.Module3DModel:
                    return _contentViewModel.GetLanguageOnLayer(go.layer);
                    break;
                
                case SceneNameConstants.ModuleDrawingTool:
                    if(go.HasComponent(out TooltipEvents tooltipEvents))
                        return tooltipEvents.GetLanguage();
                    break;
            }
            
            return _localizationModel.CurrentLanguageCultureCode;
        }
    }
}