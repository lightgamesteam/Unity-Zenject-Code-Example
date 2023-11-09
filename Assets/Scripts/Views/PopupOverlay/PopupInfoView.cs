using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class PopupInfoView : ViewBase, IAnimatedView
{
    public event Action<bool> OkCallback;
    public event Action HelpCallback;
    
    [Header("Main Panel")]
    private TextMeshProUGUI panelInfoText;

    private Toggle toggle;
    private TextMeshProUGUI toggleText;
    
    private Button okButton;
    private TextMeshProUGUI okButtonText;
    
    private Button helpButton;
    private TextMeshProUGUI helpButtonText;

    private ScrollRect scrollRect;
    
    [Header("Animation")] 
    public CanvasGroup canvasGroup;
    private RectTransform panelRect;
    public Transform mainPanel;
    private RectTransform mainPanelRect;
    
    public override void InitUiComponents()
    {
        panelRect = transform.GetComponent<RectTransform>();
        
        // main panel
        panelInfoText = transform.Get<TextMeshProUGUI>("Panel/Body/Scroll View/Viewport/Content/Text (TMP)");
        
        scrollRect = transform.Get<ScrollRect>("Panel/Body/Scroll View");
        
        toggle = transform.Get<Toggle>("Panel/Body/Toggle");
        toggleText = toggle.GetComponentInChildren<TextMeshProUGUI>();

        okButton = transform.Get<Button>("Panel/Body/Ok_Button");
        okButtonText = okButton.GetComponentInChildren<TextMeshProUGUI>();
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            HideView();
        });
        
        helpButton = transform.Get<Button>("Panel/Body/Help_Button");
        helpButtonText = helpButton.GetComponentInChildren<TextMeshProUGUI>();
        helpButton.onClick.RemoveAllListeners();
        helpButton.onClick.AddListener(() =>
        {
            HelpCallback.Invoke();
        });
        
        // animation
        canvasGroup = GetComponent<CanvasGroup>();
        mainPanel = transform.Find("Panel");
        mainPanelRect = mainPanel.GetComponent<RectTransform>();

        AsyncProcessorService.Instance.Wait(0, UpdatePanelSize);
    }

    private void UpdatePanelSize()
    {
        float sizeY = 330f;
        float panelY = panelRect.rect.height;

        panelInfoText.ForceMeshUpdate();
        
        if (panelInfoText.GetPreferredValues().y > 250f)
        {
            sizeY = 550;
        }

        if(panelInfoText.GetPreferredValues().y > panelY * 0.79f)
        {
            sizeY = panelY * 0.8f;
        }
        
        mainPanelRect.sizeDelta = new Vector2(mainPanelRect.sizeDelta.x, sizeY);
    }

    public bool GetToggleValue()
    {
        return toggle.isOn;
    }
    
    public void AddToggleListener(UnityAction<bool> onValueChanged)
    {
        toggle.onValueChanged.AddListener(onValueChanged);
    }
    
    public void RemoveToggleListener(UnityAction<bool> onValueChanged)
    {
        toggle.onValueChanged.RemoveListener(onValueChanged);
    }
    
    public void SetInfoText(string text)
    {
        panelInfoText.text = text;
    }
    
    public void SetToggleName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            toggle.gameObject.SetActive(false);
        }
        else
        {
            toggleText.text = name;
        }
    }
    
    public void SetOkButtonName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            okButton.gameObject.SetActive(false);
        }
        else
        {
            okButtonText.text = name;
        }
    }
    
    public void SetOkButtonInteractable(bool isInteractable)
    {
        okButton.interactable = isInteractable;
    }
    
    public void SetHelpButtonName(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            helpButton.gameObject.SetActive(false);
        }
        else
        {
            helpButtonText.text = name;
        }
    }

    public void ScaleText(float scaleFactor)
    {
        if(scaleFactor <= 0 || scaleFactor == 1f)
            return;
        
        gameObject.GetAllComponentsInChildren<TextMeshProUGUI>().ForEach(t => t.fontSize /= scaleFactor);
    }
    
    public void ShowView()
    {
        transform.SetAsLastSibling();
        EnableView(true);
        Signal.Fire(new FocusKeyboardNavigationSignal(canvasGroup, true));

        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutSine);

        mainPanel.localScale = Vector3.zero;
        mainPanel.DOScale(1, 0.3f).SetEase(Ease.OutSine).onComplete += () =>
        {
            okButton.Select();
            scrollRect.normalizedPosition = Vector2.one;
        };
    }

    public void HideView()
    {
        Signal.Fire(new FocusKeyboardNavigationSignal(canvasGroup, false));
        canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutSine);
        mainPanel.DOScale(0, 0.3f).SetEase(Ease.OutSine).onComplete += () =>
        {
            OkCallback.Invoke(GetToggleValue());
            ClearAction();
            Destroy(gameObject);
        };
    }
    
    private void EnableView(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }

    private void ClearAction()
    {
        OkCallback = delegate {  };
        HelpCallback = delegate {  };
    }
    
    public class Factory : PlaceholderFactory<PopupInfoView>
    {
    }
}
