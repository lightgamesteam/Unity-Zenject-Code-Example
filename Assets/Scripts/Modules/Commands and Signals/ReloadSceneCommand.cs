using UnityEngine.SceneManagement;

// TODO : Improve this 
public class ReloadSceneCommand : ICommandWithParameters
{
    private string _sceneName;
    
    public void Execute(ISignal signal)
    {
        var parameter = (ReloadSceneCommandSignal) signal;
        _sceneName = parameter.SceneName;

        SceneManager.sceneUnloaded += SceneUnloaded;
        SceneManager.UnloadSceneAsync(parameter.SceneName);
    }

    private void SceneUnloaded(Scene scene)
    {
        if (scene.name == _sceneName)
        {
            SceneManager.sceneUnloaded -= SceneUnloaded;
            SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        }
    }
}

public class ReloadSceneCommandSignal : ISignal
{
    public readonly string SceneName;
    
    public ReloadSceneCommandSignal(string sceneName)
    {
        SceneName = sceneName;
    }
}