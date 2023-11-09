using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Image))]
public class FlipOutsideLayoutDynamicDropdown : FlipOutsideLayout
{
    private CanvasGroup _canvasGroup;
    private Image _background;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup = GetComponent<CanvasGroup>();
        _background = GetComponent<Image>();
    }

    protected override void OnEnable()
    {
        _background.enabled = false;
    }

    private void OnDisable()
    {
        _background.enabled = false;
    }

    public void RepositionDropdownManually()
    {
        StartCoroutine(StartWithDelay());
    }

    private IEnumerator StartWithDelay()
    {
        _canvasGroup.alpha = 0.0f;
        yield return null;
        
        Reposition();
        
        yield return null;
        _canvasGroup.alpha = 1.0f;
        _background.enabled = true;
    }
}