using Module.Core.Attributes;
using UnityEngine;

public class TextLanguage : MonoBehaviour
{
    [SerializeField] [ShowOnly] private string language;
    
    public void SetLanguage(string _language)
    {
        language = _language;
    }
    
    public string GetLanguage()
    {
        return language;
    }
}
