using UnityEngine;

public class AddObjectToMemoryManagerSignal : ISignal
{
    public string SceneName { get; private set; }
    public Object Item { get; private set; }
   
    public AddObjectToMemoryManagerSignal(string _sceneName, Object item)
    {
        SceneName = _sceneName;
        Item = item;
    }
}