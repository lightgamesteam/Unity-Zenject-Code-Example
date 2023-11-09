using System;
using System.Linq;
using TDL.Server;
using TMPro;
using UnityEngine;

public class ContentLocalizedLabel : MonoBehaviour
{
    public static Action<string> ChangeLanguageAction = delegate {  };

    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private LocalName[] _localNames;

    private void Awake()
    {
        if(_textMeshPro == null)
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        
        ChangeLanguageAction += ChangeLanguage;
    }

    private void OnDestroy()
    {
        ChangeLanguageAction -= ChangeLanguage;
    }

    public void SetLocalNames(LocalName[] localNames)
    {
        _localNames = localNames;
    }
    
    private void ChangeLanguage(string cultureCode)
    {
        if (_localNames.Length > 0)
        {
            string local = _localNames.ToList().Find(ln => ln.Culture == cultureCode)?.Name;
            _textMeshPro.text = local;
        }
        
    }
}
