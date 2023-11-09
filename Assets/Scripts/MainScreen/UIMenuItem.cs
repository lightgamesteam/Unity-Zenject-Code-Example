using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private const float PAUSE = 0.5f;
    private const float FADE = 0.3f;

    [SerializeField] private UIMenuItem root;
    [SerializeField] private UIMenuItem menu;
    [SerializeField] private RectTransform rect;
    [SerializeField] private CanvasGroup canvasGroup;

    [SerializeField] private Vector2 offset;

    private Tween tweenHide = null;
    private Tween tweenAlpha = null;

    public void OnPointerEnter(PointerEventData eventData)
    {
        root?.KillTweenHide();
        root?.KillTweenAlpha();
        KillTweenHide();
        KillTweenAlpha();
        if (menu != this)
        {
            ActiveMenu(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        KillTweenHide();
        tweenHide = DOVirtual.DelayedCall(PAUSE, Hide);

    }

    public void KillTweenHide()
    {
        tweenHide?.Kill();
    }

    public void KillTweenAlpha()
    {
        tweenAlpha?.Kill();
    }

    private void Hide()
    {
        ActiveMenu(false);
    }

    private void ActiveMenu(bool active)
    {
        if (menu == null)
        {
            return;
        }

        menu.transform.position = root.transform.position;
        menu.rect.anchoredPosition += offset;

        menu.KillTweenAlpha();
        menu.gameObject.SetActive(true);
        if (active)
        {
            menu.canvasGroup.blocksRaycasts = false;
            tweenAlpha = menu.canvasGroup.DOFade(1, FADE);
        }
        else
        {
            menu.canvasGroup.blocksRaycasts = true;
            tweenAlpha = menu.canvasGroup.DOFade(0, FADE);
            tweenAlpha.onComplete -= HideMenuGO;
            tweenAlpha.onComplete += HideMenuGO;
        }
    }
    private void HideMenuGO()
    {
        menu.gameObject.SetActive(false);
    }
}