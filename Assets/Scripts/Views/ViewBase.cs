using UnityEngine;
using Zenject;

public abstract class ViewBase : MonoBehaviour, IView
{
    protected SignalBus Signal;

    public SignalBus SignalProperty { get => Signal; }

    [Inject]
    public void Init(SignalBus signalBus)
    {
        Signal = signalBus;
    }

    private void Awake()
    {
        InitUiComponents();
#if HIDE_MOBILE_INPUT
        HideMobileInput();
#endif
    }

    private void OnEnable()
    {
        SubscribeOnListeners();
    }

    private void OnDisable()
    {
        UnsubscribeFromListeners();
    }

    public virtual void InitUiComponents()
    {
    }

    public virtual void HideMobileInput()
    {
        
    }

    public virtual void SubscribeOnListeners()
    {
    }

    public virtual void UnsubscribeFromListeners()
    {
    }
}