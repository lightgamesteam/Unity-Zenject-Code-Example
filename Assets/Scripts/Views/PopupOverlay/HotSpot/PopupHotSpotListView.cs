using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PopupHotSpotListView : ViewBase, IAnimatedView
{
    [Header("Main Panel")]
    private TextMeshProUGUI panelName;
    private Button closeButton;
    private TooltipEvents tooltip;
    private HotSpotItemView template;
    
    [Header("Animation")] 
    public CanvasGroup canvasGroup;
    public Transform mainPanel;
    private RectTransform mainPanelRect;
    
    public override void InitUiComponents()
    {
        // main panel
        panelName = transform.Get<TextMeshProUGUI>("Panel/Panel_Name");
        template = transform.Get<HotSpotItemView>("Panel/Body/Scroll View/Viewport/Content/Template_Item");
        
        closeButton = transform.Get<Button>("Panel/Close_Button");
        tooltip = closeButton.GetComponent<TooltipEvents>();
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            HideView();
        });
        
        // animation
        canvasGroup = GetComponent<CanvasGroup>();
        mainPanel = transform.Find("Panel");
        mainPanelRect = mainPanel.GetComponent<RectTransform>();
    }
    
    public void SetPanelName(string text)
    {
        panelName.text = text;
    }

    public void SetCloseButtonTooltip(string tooltiplabel)
    {
        tooltip.SetHint(tooltiplabel);
    }
    
    public void InitHotSpotList(List<(string label, int assetId, string assetType, bool hasPlusButton)> hotSpotList, Action<int, string, bool> callback)
    {
        hotSpotList.ForEach(list =>
        {
            HotSpotItemView hs = Instantiate(template, template.transform.parent, false);
            hs.gameObject.SetActive(true);
            hs.transform.localScale = Vector3.one;
            hs.transform.SetPositionZ(template.transform.position.z);

            hs.mainButtonLabel.text = list.label;

            hs.mainButton.onClick.AddListener(() =>
            {
                callback?.Invoke(list.assetId, list.assetType, false);
                HideView();
            });

            if (list.hasPlusButton)
            {
                hs.plusButton.gameObject.SetActive(true);
                hs.plusButton.onClick.AddListener(() =>
                {
                    callback?.Invoke(list.assetId, list.assetType, true);
                    HideView();
                });
            }
            
            if (DeviceInfo.IsPCInterface())
            {
                SetTooltip(hs);
            }
        });
    }
    
    private void SetTooltip(HotSpotItemView hotSpotView)
    {
        var tooltipComponent = hotSpotView.gameObject.GetComponent<DynamicTooltipEvents>();
        if (tooltipComponent != null)
        {
            tooltipComponent.SetHint(hotSpotView.mainButtonLabel.text);
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
        mainPanel.DOScale(1, 0.3f).SetEase(Ease.OutSine).onComplete += () => {closeButton.Select();};
    }

    public void HideView()
    {
        Signal.Fire(new FocusKeyboardNavigationSignal(canvasGroup, false));
        canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutSine);
        mainPanel.DOScale(0, 0.3f).SetEase(Ease.OutSine).onComplete += () =>
        {
            Destroy(gameObject);
        };
    }
    
    private void EnableView(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }

    public class Factory : PlaceholderFactory<PopupHotSpotListView>
    {
    }
}
