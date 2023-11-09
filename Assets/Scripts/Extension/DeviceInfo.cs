using System;
using System.Globalization;
using System.Runtime.InteropServices;
using UnityEngine;

public class DeviceInfo
{
    [DllImport("__Internal")]
    private static extern bool IsMobileBrowser();

    public static string GetDeviceUID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
    
    public static bool IsMobile()
    {
#if UI_IOS

        var deviceType = iOSDeviceChecker.GetInstance().GetDeviceType();
        switch (deviceType)
        {
            case iOSDeviceChecker.DeviceType.iPhone:
                return true;
                break;
        }
        
#elif UI_ANDROID

        var deviceType = AndroidDeviceChecker.GetInstance().GetDeviceType();
        switch (deviceType)
        {
            case AndroidDeviceChecker.DeviceType.Phone:
                return true;
                break;
        }

#elif UNITY_WEBGL && !UNITY_EDITOR

        
       var s =  IsMobileBrowser();
       Debug.Log("isMOBILE = " + s);
       return s;
        
        
        
#endif
       
        return false;
    }
    
    public static bool IsTablet()
    {
        
#if UI_IOS

        var deviceType = iOSDeviceChecker.GetInstance().GetDeviceType();
        switch (deviceType)
        {
            case iOSDeviceChecker.DeviceType.iPad:
                return true;
                break;
        }
        
#elif UI_ANDROID

        var deviceType = AndroidDeviceChecker.GetInstance().GetDeviceType();
        switch (deviceType)
        {
            case AndroidDeviceChecker.DeviceType.TabletPC:
                return true;
                break;
        }
        
#endif
        
        return false;
    }

    public static bool IsChromebook()
    {
#if UI_ANDROID

        var deviceType = AndroidDeviceChecker.GetInstance().GetDeviceType();
        switch (deviceType)
        {
            case AndroidDeviceChecker.DeviceType.Chromebook:
                return true;
        }
        
#endif
        
        return false;
    }

    public static ProcessorType GetProcessorType()
    {
        if (CultureInfo.InvariantCulture.CompareInfo.IndexOf(SystemInfo.processorType, "ARM", CompareOptions.IgnoreCase) >= 0)
        {
            if (Environment.Is64BitProcess)
                return ProcessorType.ARM64;
            else
                return ProcessorType.ARM;
        }
        else
        {
            // Must be in the x86 family.
            if (Environment.Is64BitProcess)
                return ProcessorType.x86_64;
            else
                return ProcessorType.x86;
        }
    }
    
    public static bool IsWindows()
    {
#if UNITY_WINDOWS || UNITY_WINRT_10_0
        return true;
#endif
        
        return false;
    }
    
    public static bool IsAndroid()
    {
#if UNITY_ANDROID
        return true;
#endif
        
        return false;
    }
    
    public static bool IsIOS()
    {
#if UNITY_IOS
        return true;
#endif
        
        return false;
    }
    
    public static bool IsMacOS()
    {
#if UNITY_STANDALONE_OSX
        return true;
#endif
        
        return false;
    }

    public static bool IsPC()
    {
        if (IsMobile())
            return false;

        if (IsTablet())
            return false;

        if (IsChromebook())
            return false;

        return true;
    }
    
    public static bool IsPCInterface()
    {
        if (IsMobile())
            return false;

        return true;
    }

    public static DeviceFormFactor GetFormFactor()
    {
        if (IsMobile())
            return DeviceFormFactor.Mobile;

        if (IsTablet())
            return DeviceFormFactor.Tablet;

        if (IsChromebook())
            return DeviceFormFactor.Chromebook;

        if (IsPC())
            return DeviceFormFactor.PC;

        return DeviceFormFactor.Unknown;
    }
    
    public static DeviceUI GetUI()
    {
        if (IsMobile())
            return DeviceUI.Mobile;
       
        return DeviceUI.PC;
    }

    public static ScreenOrientation GetScreenOrientation()
    {
        if (Screen.height > Screen.width)
            return ScreenOrientation.Portrait;
        
        return ScreenOrientation.Landscape;
    }

    public static bool IsScreenPortrait()
    {
        ScreenOrientation so = GetScreenOrientation();
        
        if (so == ScreenOrientation.Portrait || so == ScreenOrientation.PortraitUpsideDown)
            return true;

        return false;
    }
    
    public static bool IsScreenLandscape()
    {
        ScreenOrientation so = GetScreenOrientation();

        if (so == ScreenOrientation.Landscape || so == ScreenOrientation.LandscapeLeft || so == ScreenOrientation.LandscapeRight)
            return true;

        return false;
    }

    public static UIDefine GetUIDefine()
    {
        
#if UI_IOS
        return UIDefine.UI_IOS;
        
#elif UI_ANDROID
        return UIDefine.UI_ANDROID;
        
#endif
        
        return UIDefine.UI_PC;
    }
}

public enum UIDefine
{
    UI_PC,
    UI_IOS,
    UI_ANDROID
}

public enum DeviceFormFactor
{
    Unknown,
    PC,
    Mobile,
    Tablet,
    Chromebook
}

public enum DeviceUI
{
    PC,
    Mobile
}

public enum ProcessorType
{
    ARM64,
    ARM,
    x86_64,
    x86
}