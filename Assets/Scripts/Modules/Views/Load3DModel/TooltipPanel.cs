using System.Collections.Generic;
using TDL.Constants;
using TDL.Models;
using TMPro;
using UnityEngine;
using Zenject;

public class TooltipPanel : MonoBehaviour
{
    [Inject] private AccessibilityModel _accessibilityModel;

    public static TooltipPanel Instance;
    private float _defaultFontSize;
    
    private TextMeshProUGUI _tooltipText;
    private RectTransform rectTransform;
    private Canvas myCanvas;

    private Camera camera;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        
        FindCamera();

        _tooltipText = GetComponentInChildren<TextMeshProUGUI>();
        _defaultFontSize = _tooltipText.fontSize;
        gameObject.SetActive(false);
    }

    private void FindCamera()
    {
        if (!rectTransform)
            rectTransform = GetComponent<RectTransform>();

        if (!myCanvas)
            myCanvas = rectTransform.GetMyCanvas();

        if (!camera)
            camera = myCanvas.worldCamera;
    }

    public void FeedTooltipEvents(string cultureCode, Dictionary<string, string> translation)
    {
       gameObject.GetAllInScene<TooltipEvents>().ForEach(tte =>
       {
           if (translation.ContainsKey(tte.GetKey())) 
               tte.SetHintAndLanguage(translation[tte.GetKey()], cultureCode);
       });
    }

    public void SetTooltipText(string text)
    {
        _tooltipText.text = text;
    }
    
    public void SetSafeDisableTooltip(string text)
    {
        if (!gameObject.activeSelf) { return; }

        if (_tooltipText.text.Equals(text))
            gameObject.SetActive(false);
    }
    
    public void SetEnableTooltip(string text)
    {
        FindCamera();
        SetPosition();
        UpdateFontSize();
        _tooltipText.text = text;
        gameObject.SetActive(true);
    }
    
    public void SetDisableTooltip()
    {
        if (this)
        {
            gameObject.SetActive(false);
        }
    }

    private void UpdateFontSize()
    {        
        _tooltipText.fontSize = _defaultFontSize;
        var currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
        if (currentFontSize != AccessibilityConstants.FontSizeMedium150)
        {
            _tooltipText.fontSize = Mathf.RoundToInt(_tooltipText.fontSize / _accessibilityModel.ModulesFontSizeScaler);
        }
    }
    
    private void Update()
    {
        SetPosition();
    }
    
    private void SetPosition()
    {
        if (camera)
        {
            var screenPoint = Input.mousePosition;
            screenPoint.z = myCanvas.planeDistance; //distance of the plane from the camera
        
            var pos = camera.ScreenToWorldPoint(screenPoint);
            var dis = GetDisplace(screenPoint);

            transform.position = new Vector3(pos.x + dis, pos.y, transform.position.z); 
        }
    }

    private float GetDisplace(Vector3 mousePos)
    {
        var scf = 1f / Screen.width;
        
        if (mousePos.x > Screen.width - (rectTransform.rect.xMax - rectTransform.rect.xMin  + 35) * myCanvas.scaleFactor)
        {
            rectTransform.pivot = new Vector2(1, 1);
            return -5 * scf * myCanvas.scaleFactor * myCanvas.planeDistance;
        }

        rectTransform.pivot = new Vector2(0, 1);
        return 50 * scf * myCanvas.scaleFactor * myCanvas.planeDistance;
    }
}