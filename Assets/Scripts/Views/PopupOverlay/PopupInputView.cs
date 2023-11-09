using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PopupInputView : ViewBase, IAnimatedView
{
    public event Action OnCancel;
    public event Action<string> OnSubmit;
    
    [Header("Main Panel")]
    private TextMeshProUGUI panelTitleText;
    
    private InputField InputField;
    
    private Button closeButton;
    private TextMeshProUGUI closeButtonText;

    private Button okButton;
    private TextMeshProUGUI okButtonText;
    
    [Header("Animation")] 
    public CanvasGroup canvasGroup;
    public Transform mainPanel;
    
    public override void InitUiComponents()
    {
        // main panel
        panelTitleText = transform.Get<TextMeshProUGUI>("Panel/Top/Title");
        Debug.Log("init");
        
        InputField = transform.Get<InputField>("Panel/Body/InputField");
        InputField.onValueChanged.RemoveAllListeners();
        InputField.onValueChanged.AddListener(InputValueChanged);
        
        closeButton = transform.Get<Button>("Panel/Body/Cancel_Button");
        closeButtonText = closeButton.GetComponentInChildren<TextMeshProUGUI>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            OnCancel.Invoke();
            HideView();
        });
        
        okButton = transform.Get<Button>("Panel/Body/Ok_Button");
        okButtonText = okButton.GetComponentInChildren<TextMeshProUGUI>();
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            OnSubmit.Invoke(InputField.text);
            HideView();
        });
        
        // animation
        canvasGroup = GetComponent<CanvasGroup>();
        mainPanel = transform.Find("Panel");
    }

    private void InputValueChanged(string value)
    {
        if (value.Length < 1)
            okButton.interactable = false;
        else
            okButton.interactable = true;
    }

    public void SetInputFieldPlaceholderTitle(string name)
    {
        InputField.placeholder.GetComponent<Text>().text = name;
    }

    public void SetInputReadonly(bool isReadonly)
    {
        InputField.readOnly = isReadonly;
    }
    
    public void SetPanelName(string name)
    {
        panelTitleText.text = name;
    }
    
    public void SetCancelButtonName(string name)
    {
        closeButtonText.text = name;
    }
    
    public void SetOkButtonName(string name)
    {
        okButtonText.text = name;
    }

    public void SetInputValue(string value)
    {
        InputField.text = value;
    }
    
    public void ScaleText(float scaleFactor)
    {
        if(scaleFactor <= 0 || scaleFactor == 1f)
            return;

        gameObject.GetAllComponentsInChildren<Text>().ForEach(t => t.fontSize = (int) (t.fontSize / scaleFactor));
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
        mainPanel.DOScale(1, 0.3f).SetEase(Ease.OutSine).onComplete += () => {InputField.Select();};
    }

    public void HideView()
    {
        Signal.Fire(new FocusKeyboardNavigationSignal(canvasGroup, false));
        ClearAction();
        canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutSine);
        mainPanel.DOScale(0, 0.3f).SetEase(Ease.OutSine).onComplete += () =>  Destroy(gameObject);
    }
    
    private void EnableView(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }

    private void ClearAction()
    {
        OnCancel = delegate {  };
        OnSubmit = delegate {  };
    }
    
    public class Factory : PlaceholderFactory<PopupInputView>
    {
    }
}
