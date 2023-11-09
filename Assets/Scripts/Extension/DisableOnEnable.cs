using System;
using TDL.Constants;
using UnityEngine;

public class DisableOnEnable : MonoBehaviour
{
    public bool onAwake = true;
     [Header("Disable:")] [Space(10)]
    public bool onAll = true;
    public bool onChromebook = false;
    public bool onAndroid = false;
    public bool onX86Android = false;
    public bool onAndroidMobile = false;
    public bool onAndroidTablet = false;
    public bool onIOS = false;
    public bool onIOSMobile = false;
    public bool onIOSTablet = false;
    public bool onWindows = false;
    public bool onMacOS = false;
    public bool onARMode = false;

    private void Awake()
    {
        if(!this.enabled)
            return;
        
        if(onAwake && NeedDisable())
            gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if(NeedDisable())
            gameObject.SetActive(false);
    }

    private bool NeedDisable()
    {
        if (onAll)
            return true;

        if(DeviceInfo.IsChromebook() && onChromebook)
            return true;
        if(DeviceInfo.IsAndroid() && onAndroid)
            return true;
        if (onX86Android && DeviceInfo.IsAndroid())
        {
            if(DeviceInfo.GetProcessorType().Equals(ProcessorType.x86_64) || DeviceInfo.GetProcessorType().Equals(ProcessorType.x86))  
                return true;
        }
        if(DeviceInfo.IsAndroid() && DeviceInfo.IsMobile() && onAndroidMobile)
            return true;
        if(DeviceInfo.IsAndroid() && DeviceInfo.IsTablet() && onAndroidTablet)
            return true;
        if(DeviceInfo.IsIOS() && onIOS)
            return true;
        if(DeviceInfo.IsIOS() && DeviceInfo.IsMobile() && onIOSMobile)
            return true;
        if(DeviceInfo.IsIOS() && DeviceInfo.IsTablet() && onIOSTablet)
            return true;
        if(DeviceInfo.IsWindows() && onWindows)
            return true;
        if(DeviceInfo.IsMacOS() && onMacOS)
            return true;
        if(PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.ARmodeSettings) && onARMode)
            return true;
        
        return false;
    }
}
