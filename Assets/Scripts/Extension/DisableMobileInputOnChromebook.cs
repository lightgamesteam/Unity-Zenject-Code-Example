using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisableMobileInputOnChromebook : MonoBehaviour
{
    private void Awake()
    {
        if (DeviceInfo.IsChromebook())
            DisableMobileInput();
    }

    public void DisableMobileInput()
    {
        if (gameObject.HasComponent(out InputField input))
            input.shouldHideMobileInput = true;
        
        if (gameObject.HasComponent(out TMP_InputField inputTMP))
            inputTMP.shouldHideMobileInput = true;
    }
}
