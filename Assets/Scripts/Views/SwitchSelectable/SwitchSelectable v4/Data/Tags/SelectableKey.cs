using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SelectableKey : MonoBehaviour
{
    public KeyCode key = KeyCode.Escape;

    [SerializeField]
    private Toggle _toggle;

    private void OnEnable()
    {
        _toggle = GetComponent<Toggle>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (_toggle.isOn)
                _toggle.isOn = false;
        }
    }
}