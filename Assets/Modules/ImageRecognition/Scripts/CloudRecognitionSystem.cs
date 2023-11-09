using TDL.Signals;
using UnityEngine;
using UnityEngine.SceneManagement;
#if !UNITY_WEBGL
using Vuforia;
#endif
using Zenject;

public class CloudRecognitionSystem : MonoBehaviour
{
    [Inject] private readonly SignalBus _signal;
#if !UNITY_WEBGL
    private CloudRecoBehaviour mCloudRecoBehaviour;

   
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        VuforiaRuntime.Instance.InitVuforia();
    }

    protected virtual void Start()
    {
        mCloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
        
        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.RegisterOnInitializedEventHandler(OnInitialized);
            mCloudRecoBehaviour.RegisterOnInitErrorEventHandler(OnInitError);
            mCloudRecoBehaviour.RegisterOnUpdateErrorEventHandler(OnUpdateError);
            mCloudRecoBehaviour.RegisterOnStateChangedEventHandler(OnStateChanged);
            mCloudRecoBehaviour.RegisterOnNewSearchResultEventHandler(OnNewSearchResult);
            mCloudRecoBehaviour.CloudRecoEnabled = true;
        }
    }

    protected virtual void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (mCloudRecoBehaviour)
        {
            mCloudRecoBehaviour.UnregisterOnInitializedEventHandler(OnInitialized);
            mCloudRecoBehaviour.UnregisterOnInitErrorEventHandler(OnInitError);
            mCloudRecoBehaviour.UnregisterOnUpdateErrorEventHandler(OnUpdateError);
            mCloudRecoBehaviour.UnregisterOnStateChangedEventHandler(OnStateChanged);
            mCloudRecoBehaviour.UnregisterOnNewSearchResultEventHandler(OnNewSearchResult);
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Equals(gameObject.scene.name))
        {
            _signal.Fire(new PopupOverlaySignal(false));
        }
    }

    public void OnInitialized(TargetFinder targetFinder)
    {
        Debug.Log ("Cloud Reco initialized");
    }

    public void OnInitError(TargetFinder.InitState initError)
    {
        Debug.Log ("Cloud Reco init error " + initError);
    }

    public void OnUpdateError(TargetFinder.UpdateState updateError)
    {
        Debug.Log ("Cloud Reco update error " + updateError);
    }

    public void OnStateChanged(bool scanning)
    {
        
    }

    public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
    {
        TargetFinder.CloudRecoSearchResult cloudRecoSearchResult = (TargetFinder.CloudRecoSearchResult)targetSearchResult;
        
        mCloudRecoBehaviour.CloudRecoEnabled = false;
        
        _signal.Fire(new ImageRecognitionResponseSignal(true, cloudRecoSearchResult.TargetName, cloudRecoSearchResult.MetaData));
        
        UnloadScene();
    }

    public void CloseModule()
    {
        _signal.Fire(new ImageRecognitionResponseSignal(false, string.Empty, string.Empty));
        UnloadScene();
    }

    private void UnloadScene()
    {
        SceneManager.UnloadSceneAsync(gameObject.scene.name);
    }
#endif
}

