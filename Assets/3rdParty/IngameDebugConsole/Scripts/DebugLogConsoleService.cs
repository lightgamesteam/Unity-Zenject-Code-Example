using System.IO;
using IngameDebugConsole;
using TDL.Constants;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using Zenject;

public class DebugLogConsoleService : IInitializable
{
    [Inject] private readonly SignalBus _signal;
    [Inject] private readonly ApplicationSettingsInstaller.ServerSettings _serverSettings;
    [Inject] private ScreenRecorderManager _screenRecorder;
    [Inject] private ServerService _serverService;
    
    public void Initialize()
    {
        DebugLogConsole.AddCommandInstance("server", "Current Server", "Server", this);
        DebugLogConsole.AddCommandInstance("stage_server", "Select Stage Server [isStageServer]", "StageServer", this);
        DebugLogConsole.AddCommandInstance("login", "Login [userName] [userPassword]", "Login", this);
        
        DebugLogConsole.AddCommandInstance( "cp", "Clear PlayerPrefs", "ClearPlayerPrefs", this);
        
        DebugLogConsole.AddCommandInstance( "cache", "Get Path To App Cache Folder", "AppCache", this);
        DebugLogConsole.AddCommandInstance( "oc", "Open App Cache Folder", "OpenAppCache", this);
        DebugLogConsole.AddCommandInstance( "cc", "Clear App Cache", "ClearAppCache", this);
        
        DebugLogConsole.AddCommandInstance( "gmic", "Get Microphones", "GetMicrophones", this);
        DebugLogConsole.AddCommandInstance( "smic", "Set Default Microphone", "SetDefaultMicrophone", this);
        DebugLogConsole.AddCommandInstance( "rmic", "Reset Default Microphone", "ResetDefaultMicrophone", this);
        DebugLogConsole.AddCommandInstance( "tmic", "Test Microphone (5 sec)", "TestMicrophone", this);
        DebugLogConsole.AddCommandInstance( "start_mic", "Start Test Microphone", "StartMicrophone", this);
        DebugLogConsole.AddCommandInstance( "stop_mic", "Stop Test Microphone", "StopMicrophone", this);
        
        DebugLogConsole.AddCommandInstance( "ate", "Accept Term (need login)", "AcceptTerm", this);
        DebugLogConsole.AddCommandInstance( "dte", "Decline Term (need login)", "DeclineTerm", this);
        DebugLogConsole.AddCommandInstance( "vte", "Validate Term (need login)", "ValidateTerm", this);
        
        DebugLogConsole.AddCommandInstance( "duid", "Device UID", "DeviceUID", this);
        
        DebugLogConsole.AddCommandInstance( "gar", "Get Available Resources From Server", "GetAvailableResourcesFromServer", this);
        
        DebugLogConsole.AddCommandInstance( "sdr", "Set Is On Debug Request", "SetIsOnDebugRequest", this);
        
        if(DeviceInfo.IsIOS())
            DebugLogConsole.AddCommandInstance( "dt", "Device Type: 0 - Default, 1 - IPhone, 2- IPad", "SetDeviceType", this);
        
        if(DeviceInfo.IsAndroid())
            DebugLogConsole.AddCommandInstance( "dt", "Device Type: 0 - Default, 1 - Phone, 2 - TabletPC, 3 - Chromebook", "SetDeviceType", this);
        
        DebugLogConsole.AddCommandInstance( "quit", "Application Quit", "ApplicationQuit", this);

        ConsoleUWP();
    }
    
    private void GetAvailableResourcesFromServer()
    {
        _signal.Fire<GetAvailableResourcesCommandSignal>();
    }

    private void DeviceUID()
    {
        Debug.Log($"DeviceUID: {DeviceInfo.GetDeviceUID()}");
    }
    
    private void ValidateTerm()
    {
        _serverService.ValidateTerm(b => Debug.Log($"Term Is Accept = {b}"));
    }
    
    private void AcceptTerm()
    {
        _serverService.AcceptTerm(isShowLog:true);
    }
    
    private void DeclineTerm()
    {
        _serverService.AcceptTerm(false, true);
    }

    private void SetDeviceType(int type)
    {
        if (type == 0)
        {
            PlayerPrefs.DeleteKey(PlayerPrefsKeyConstants.DeviceType);
        }
        else
        {
            PlayerPrefs.SetInt(PlayerPrefsKeyConstants.DeviceType, type);
        }
    }
    
    private void SetIsOnDebugRequest(bool isDebugOn)
    {
        PlayerPrefsExtension.SetBool(PlayerPrefsKeyConstants.DebugRequest, isDebugOn);
    }

    private void ConsoleUWP()
    {
#if UNITY_WSA && !UNITY_EDITOR
        DebugLogConsole.AddCommandInstance( "video_status", "Video Recording Status", "VideoRecordingStatus", this);
#endif
    }
    
    public void VideoRecordingStatus()
    {
        string status = "null";

#if UNITY_WSA && !UNITY_EDITOR
        status = UWP.VideoRecorder.GetStatus();
#endif
        
        Debug.Log("Video Recording Status \n" + status);
    }
    
    private void GetMicrophones()
    {
#if !UNITY_WEBGL
        int i = 0;
        string devices = "Get Audio Devices: \n";
        foreach (string device in Microphone.devices)
        {
            devices += $"ID: {i} - Name: {device} \n";
            i++;
        }
        
        Debug.Log(devices);
#endif
    }
    
    private void SetDefaultMicrophone(int id)
    {
#if !UNITY_WEBGL
        if (id < Microphone.devices.Length)
        {
            string deviceName = Microphone.devices[id];
            PlayerPrefsExtension.SetString(PlayerPrefsKeyConstants.DefaultMicrophone, deviceName);
            Debug.Log($"Set Default Microphone > Successful > {id} - Name: {deviceName}");
        }
        else
        {
            Debug.Log($"Set Default Microphone > Error > Wrong ID: {id}");
        }
#endif
    }

    private AudioClip _microAudioClip;
    private int _microID = -1;
    private void StartMicrophone(int id)
    {
#if !UNITY_WEBGL
        if (id < Microphone.devices.Length)
        {
            Debug.Log($"Start Microphone > ID: {id}");

            _microID = id;
            _microAudioClip = Microphone.Start(Microphone.devices[_microID], false, 60, 44100);
            _screenRecorder.gameObject.GetComponent<AudioSource>().clip = _microAudioClip;
            _screenRecorder.gameObject.GetComponent<AudioSource>().PlayDelayed(0.1f);
        }
#endif
    }
    
    private void TestMicrophone(int id)
    {
#if !UNITY_WEBGL
        if (id < Microphone.devices.Length)
        {
            Debug.Log($"Start Test Microphone > ID: {id}");

            _microAudioClip = Microphone.Start(Microphone.devices[id], false, 10, 44100);
            _screenRecorder.gameObject.GetComponent<AudioSource>().clip = _microAudioClip;
            _screenRecorder.gameObject.GetComponent<AudioSource>().PlayDelayed(0.1f);
            
            AsyncProcessorService.Instance.Wait(5f, () =>
            {
                Debug.Log($"Stop Test Microphone > ID: {id}");
                Microphone.End(Microphone.devices[id]);
            });
        }
        else
        {
            Debug.Log($"Test Microphone > Wrong ID: {id}");
        }
#endif
    }
    
    private void StopMicrophone()
    {
#if !UNITY_WEBGL
        if (_microID > -1)
        {
            Debug.Log($"Stop Microphone > ID: {_microID}");

            Microphone.End(Microphone.devices[_microID]);
            _microAudioClip = null;
            _microID = -1;
        }
#endif
    }
    
    private void ResetDefaultMicrophone()
    {
        PlayerPrefsExtension.SetString(PlayerPrefsKeyConstants.DefaultMicrophone, string.Empty);
        Debug.Log($"Reset Default Microphone > Successful");
    }

    public void Login(string userName, string userPassword)
    {
        _signal.Fire(new LoginClickCommandSignal(userName, userPassword, false));
    }
    
    public void Server()
    {
        Debug.Log($"Current Server: {_serverSettings.ApiUrl}");
    }
    
    public void StageServer(bool isStageServer)
    {
        _serverSettings.UseApiTestUrl = isStageServer;
        Debug.Log($"Server set to: {_serverSettings.ApiUrl}");
    }
    
    public void ClearAppCache()
    {
        DirectoryInfo di = new DirectoryInfo(GetPathToCommonCacheFolder());

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete(); 
        }
			
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete(); 
            }
        }
    }

    public void OpenAppCache()
    {
        Application.OpenURL($"file://{GetPathToCommonCacheFolder()}");
    }

    public void AppCache()
    {
        Debug.Log(GetPathToCommonCacheFolder());
    }

    private string GetPathToCommonCacheFolder()
    {
        return Path.Combine(Application.persistentDataPath, CacheConstants.CacheFolder);
    }
		
    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
		
    public void ApplicationQuit()
    {
        Application.Quit();
    }
}
