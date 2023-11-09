using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldRegexCheck : MonoBehaviour
{
    string removableChars = $"[{Regex.Escape(@"\\/:*?""<>|{}")}]" ;

    private InputField _tmp;
    private string newText;
    
    void Awake()
    {
        if (!_tmp)
            _tmp = GetComponent<InputField>();
    }

    private void OnEnable()
    {
        _tmp.onValueChanged.AddListener(RegexRemove);
    }

    private void OnDisable()
    {
        _tmp.onValueChanged.RemoveListener(RegexRemove);
    }

    private void RegexRemove(string value)
    {
        _tmp.text = Regex.Replace(value, removableChars, "");
    }
}
