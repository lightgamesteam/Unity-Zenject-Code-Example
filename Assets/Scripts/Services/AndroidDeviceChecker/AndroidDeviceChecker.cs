using TDL.Constants;
using UnityEngine;

public class AndroidDeviceChecker
{
    
#if UI_ANDROID

    public enum DeviceType
    {
        Phone,
        TabletPC,
        Chromebook
    }

    private static AndroidDeviceChecker _instance;
    private AndroidJavaObject _javaObject;
    private AndroidJavaObject _context;
    private DeviceType _currentDeviceType;

    private AndroidDeviceChecker()
    {
#if UI_ANDROID && !UNITY_EDITOR
            var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _context = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
            _javaObject = new AndroidJavaObject("no.threedimensionallearning.tdlandroiddevicechecker.EntryPointClass");
            _currentDeviceType = (DeviceType) _javaObject.Call<int>("GetPlatform", _context);
#else
        _currentDeviceType = DeviceType.Phone;
#endif
    }
    
    public static AndroidDeviceChecker GetInstance()
    {
        if (_instance == null)
        {
            _instance = new AndroidDeviceChecker();
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
                    return DeviceType.Phone;
                break;
                
                case 2:
                    return DeviceType.TabletPC;
                    break;
                
                case 3:
                    return DeviceType.Chromebook;
                    break;
            }
        }
        
        if (Application.isEditor)
            return DeviceType.Phone;
        
        return _currentDeviceType;
    }

#endif

}