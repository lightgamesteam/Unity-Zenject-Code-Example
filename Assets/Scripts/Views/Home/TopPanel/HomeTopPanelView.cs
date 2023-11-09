using UnityEngine;
using UnityEngine.UI;

public class HomeTopPanelView : ViewBase
{
    [SerializeField] 
    private Toggle _hamburgerToggle;

    public override void SubscribeOnListeners()
    {
        _hamburgerToggle.onValueChanged.AddListener(OnHamburgerClick);
    }

    public override void UnsubscribeFromListeners()
    {
        _hamburgerToggle.onValueChanged.RemoveAllListeners();
    }

    public void ChangeToggleState(bool status)
    {
        if (_hamburgerToggle.isOn != status)
        {
            _hamburgerToggle.isOn = status;
        }
    }

    private void OnHamburgerClick(bool isEnabled)
    {
        Signal.Fire(new LeftMenuClickViewSignal(isEnabled));
    }
}