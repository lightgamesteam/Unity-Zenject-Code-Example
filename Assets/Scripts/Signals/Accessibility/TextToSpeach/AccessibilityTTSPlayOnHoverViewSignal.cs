using UnityEngine;

public class AccessibilityTTSPlayOnHoverViewSignal : ISignal
{
    public GameObject ObjectWithAudio { get; private set; }

    public AccessibilityTTSPlayOnHoverViewSignal(GameObject objectWithAudio)
    {
        ObjectWithAudio = objectWithAudio;
    }
}