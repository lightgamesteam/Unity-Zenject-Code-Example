using System.Runtime.InteropServices;
using TDL.Constants;
using UnityEngine;

public class iOSDeviceChecker 
{
    
#if UI_IOS
    
    public enum DeviceType
    {
        iPhone,
        iPad
    }
    
    [DllImport ("__Internal")]
    private static extern int GetiOSDeviceType();
    
    private static iOSDeviceChecker _instance;
    private readonly DeviceType _currentDeviceType;

    private iOSDeviceChecker()
    {
        if (Application.isEditor)
        {
            _currentDeviceType = DeviceType.iPhone;
        }
        else
        {
            _currentDeviceType = (DeviceType) GetiOSDeviceType();
        }
    }
    
    public static iOSDeviceChecker GetInstance()
    {
        if (_instance == null)
        {
            _instance = new iOSDeviceChecker();
        }

        return _instance;
    }

    public DeviceType GetDeviceType()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKeyConstants.DeviceType))
        {
            int type = PlayerPrefs.GetInt(PlayerPrefsKeyConstants.DeviceType);
            
            switch (type)
            {
                case 1:
                    return DeviceType.iPhone;
                    break;
                
                case 2:
                    return DeviceType.iPad;
                    break;
            }
        }
        
        return _currentDeviceType;
    }
    
#endif
    
}