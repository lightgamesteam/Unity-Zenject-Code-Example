using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PopupWarningARView : ViewBase, IAnimatedView
{
    public event Action OkCallback;
    
    [Header("Main Panel")]
    private Button okButton;
    private TextMeshProUGUI okButtonText;
    private Image warningImage;
    
    [Header("Animation")] 
    public CanvasGroup canvasGroup;
    public Transform mainPanel;
    private RectTransform mainPanelRect;
    
    public override void InitUiComponents()
    {
        // main panel
        warningImage = transform.Get<Image>("Panel/Body/Image");
        okButton = transform.Get<Button>("Panel/Body/Ok_Button");
        okButtonText = okButton.GetComponentInChildren<TextMeshProUGUI>();
        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            HideView();
        });
        
        // animation
        canvasGroup = GetComponent<CanvasGroup>();
        mainPanel = transform.Find("Panel");
        mainPanelRect = mainPanel.GetComponent<RectTransform>();
    }

    public void SetWarningSprite(Sprite sprite)
    {
        warningImage.sprite = sprite;
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
        };
    }

    public void HideView()
    {
        Signal.Fire(new FocusKeyboardNavigationSignal(canvasGroup, false));
        canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutSine);
        mainPanel.DOScale(0, 0.3f).SetEase(Ease.OutSine).onComplete += () =>
        {
            OkCallback.Invoke();
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
    }
    
    public class Factory : PlaceholderFactory<PopupWarningARView>
    {
    }
}
