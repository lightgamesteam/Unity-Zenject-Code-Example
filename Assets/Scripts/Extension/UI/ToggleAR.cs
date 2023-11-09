using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleAR : MonoBehaviour
{
    [SerializeField] private  TextMeshProUGUI _title;
    [SerializeField] private  TextMeshProUGUI _titleIcon;
    [SerializeField] private  Image _icon;
    [SerializeField] private  Image _selectedModelIcon;
    
    [SerializeField] private string _keyTrue;
    [SerializeField] private string _keyFalse;

    private Toggle _toggle;

    public Toggle toggle => _toggle;
    
    private string _textTrue;
    private string _textFalse;
    
    [Space]
    public Toggle.ToggleEvent onValueChanged = new Toggle.ToggleEvent();

    void Awake()
    {
        _toggle = GetComponent<Toggle>();
    }
    
    private void OnEnable()
    {
        _toggle.onValueChanged.RemoveAllListeners();
        _toggle.onValueChanged.AddListener(ChangeValue);
    }

    public void UpdateTitle()
    {
        if(_toggle == null)
            _toggle = GetComponent<Toggle>();
        
        ChangeValue(_toggle.isOn);
    }
    
    private void ChangeValue(bool value)
    {
        onValueChanged.Invoke(value);

        ChangeTitle(value);
    }

    private void ChangeTitle(bool value)
    {
        if (_title == null)
        {
            return;
        }
        
        if (value)
        {
            _title.text = _textTrue;
        }
        else
        {
            _title.text = _textFalse;
        }
    }

    public void SetSelectedModelIcon(bool isSelected)
    {
        _selectedModelIcon.gameObject.SetActive(isSelected);
    }

    public void SetValue(bool value, bool sendCallback)
    {
        _toggle.SetValue(value, sendCallback);
        ChangeTitle(value);
    }

    public string GetKeyTrue()
    {
        return _keyTrue;
    }
    
    public string GetKeyFalse()
    {
        return _keyFalse;
    }

    public void SetLocalization(string valueTrue, string valueFalse)
    {
        _textTrue = valueTrue;
        _textFalse = valueFalse;
        UpdateTitle();
    }

    public void SetTitleIcon(string titleIcon)
    {
        _titleIcon.text = titleIcon;
    }

    public void SetIconColor(Color color)
    {
        if (_icon)
            _icon.color = color;
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
