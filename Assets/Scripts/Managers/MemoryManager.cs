using System;
using System.Collections.Generic;
using TDL.Signals;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

public class MemoryManager : IInitializable, IDisposable
{
    public static MemoryManager Instance;
    
    [Inject] private readonly SignalBus _signal;
    [Inject] private AsyncProcessorService _asyncProcessor;

    private Dictionary<string, List<Object>> _objectsInMemory = new Dictionary<string, List<Object>>();

    public void Initialize()
    {
        if (Instance == null)
            Instance = this;
        
        SceneManager.sceneUnloaded += SceneUnloaded;
        
        _signal.Subscribe<AddObjectToMemoryManagerSignal>(AddObjectToMemoryManager);
    }
    
    public void Dispose()
    {
        SceneManager.sceneUnloaded -= SceneUnloaded;
    }

    private void SceneUnloaded(Scene scene)
    {
        string sName = scene.name;
        _asyncProcessor.Wait(0.2f, () => DestroyObjectsInMemory(sName));
    }

    public void AddObjectToMemoryManager(string sceneName, Object item)
    {
        if(!_objectsInMemory.ContainsKey(sceneName))
            _objectsInMemory.Add(sceneName, new List<Object>());
            
        _objectsInMemory[sceneName].Add(item);
    }

    private void AddObjectToMemoryManager(ISignal _signal)
    {
        AddObjectToMemoryManagerSignal value = (AddObjectToMemoryManagerSignal)_signal;;

        AddObjectToMemoryManager(value.SceneName, value.Item);
    }
    
    private void DestroyObjectsInMemory(string sceneName)
    {
        if (_objectsInMemory.ContainsKey(sceneName))
        {
            string deleted = string.Empty;
            int i = 0;
            _objectsInMemory[sceneName].ForEach(obj =>
            {
                if (obj != null)
                {
                    i++;
                    deleted += $"\n[{i}] > {obj} ";
                     Object.DestroyImmediate(obj, true);
                }
            });
            
            _signal.Fire(new ShowDebugLogCommandSignal($"||| MemoryManager > DestroyObjectsInMemory = {_objectsInMemory[sceneName].Count} {deleted} \n"));

            _objectsInMemory.Remove(sceneName);
        }

        Resources.UnloadUnusedAssets();
    }
}
