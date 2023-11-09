using UnityEngine;

public class DisableUWPIsCantRecord : MonoBehaviour
{
    private void Awake()
    {
        if(!this.enabled)
            return;
        
        DisableThis();
    }

    private void OnEnable()
    {
        DisableThis();
    }

    private void DisableThis()
    {
        if(NeedDisable())
            gameObject.SetActive(false);
    }

    private bool NeedDisable()
    {
        
#if UNITY_WSA && !UNITY_EDITOR
        if (!UWP.VideoRecorder.IsCanRecord())
            return true;
#endif
        
        return false;
    }
}
