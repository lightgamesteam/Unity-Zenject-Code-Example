using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ClickOutside : MonoBehaviour
{
    public bool checkStatusOnAwake = false;

    private bool _isPined = false;
    public bool IsPined => _isPined;
    private ClickOutsideTrigger _trigger;
    private Toggle _toggle;

    private string TriggerName => $"{myCanvas.name}_ClickOutsideTrigger";
    private Canvas myCanvas;

    public void Awake()
    {
        _toggle = GetComponent<Toggle>();
        myCanvas = transform.GetComponent<RectTransform>().GetMyCanvas();
        
        SetupClickOutside();
    }

    public void SetPinStatus(bool isPined)
    {
        _isPined = isPined;
        _lastPinStatus = _isPined;
        
        if (!_isPined && _toggle.isOn)
        {
            SetTrigger(true);
        }
    }

    private bool _lastPinStatus;
    public void SetTemporaryPinStatus(bool isPined)
    {
        if (isPined)
        {
            _lastPinStatus = _isPined;
            _isPined = true;
        }
        else
        {
            _isPined = _lastPinStatus;
        }
        
        if (!_isPined && _toggle.isOn)
        {
            SetTrigger(true);
        }
    }
    
    private void SetupClickOutside ()
    {
        if (_trigger == null)
            _trigger = myCanvas.transform.GetChildrenByName(TriggerName)?.GetComponent<ClickOutsideTrigger>();

        if (_trigger == null)
        {
            _trigger = new GameObject(TriggerName).AddComponent<ClickOutsideTrigger>();

            Image tempImage = _trigger.gameObject.AddComponent<Image>();
            tempImage.color = new Color(0f, 0f, 0f, 0f);
            
            //tempImage.color = new Color(0.9f, 0.5f, 0.5f, 0.7f); // Test <---------------------
        }
        
        _toggle.onValueChanged.AddListener(SetTrigger);
         
        RectTransform tempTransform = _trigger.GetComponent<RectTransform>();
        tempTransform.SetParent(myCanvas.transform, false);
        tempTransform.SetSiblingIndex(myCanvas.transform.GetChild(0).GetSiblingIndex()); // put it right beind this panel in the hierarchy
        
        tempTransform.anchorMin = new Vector2(0f, 0f);
        tempTransform.anchorMax = new Vector2(1f, 1f);
        tempTransform.offsetMin = new Vector2(0f, 0f);
        tempTransform.offsetMax = new Vector2(0f, 0f);

        if (checkStatusOnAwake && _toggle.isOn)
        {
            AsyncProcessorService.Instance.Wait(0, () => SetTrigger(true));
        }
        else
        {
            _trigger.gameObject.SetActive(false);
        }
    }
    
    public void SetTrigger (bool value)
    {
        if (value)
        {
            _trigger.AddTrigger(this, Clicked);
        }
        else
        {
            _trigger.RemoveTrigger(this);
        }
    }
    
    public void Clicked ()
    {
        if (!IsPined)
        {
            _toggle.isOn = false;
        }
        else
        {
            SetTrigger(false);
        }
    }
}