using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("UI/Copy TextMeshPro - Event Extension")]
public class CopyTextMeshPro : MonoBehaviour
{
    public TextMeshProUGUI copyFrom;

    [Serializable]
    public class OnValueChanged : UnityEvent<string>
    {
        
    }
    
    public OnValueChanged onValueChanged = new OnValueChanged();

    public void CopyText()
    {
        onValueChanged.Invoke(copyFrom.text);
    }
    
    public void CopyText(bool status)
    {
        if(status)
            onValueChanged.Invoke(copyFrom.text);
    }
}