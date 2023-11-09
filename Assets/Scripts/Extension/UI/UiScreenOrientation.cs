using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class UiScreenOrientation : MonoBehaviour
{
   #if UNITY_EDITOR
    public bool emulateInEditor = true;

    private void OnValidate()
    {    
        if (emulateInEditor && this.enabled && gameObject.activeSelf)
        {
            EmulateScreenOrientation();
        }
    }
#endif
    
    [Header("Portrait")]
    public float bottomSizeY = 200;
    
    [Header("Landscape")]
    public float rightSizeX = 250;
    public float positionFromTop = 110f;

    private DeviceOrientation lastScreenOrientation;
    [SerializeField] private RectTransform rect;

    private void Awake()
    {
        if(!this.enabled || !gameObject.activeSelf)
            return;
        
        rect = GetComponent<RectTransform>();

        if (Screen.width < Screen.height)
        {
            lastScreenOrientation = DeviceOrientation.Portrait;
            CnangeUI(lastScreenOrientation);
        }
        else 
        {
            lastScreenOrientation = DeviceOrientation.LandscapeLeft;
            CnangeUI(lastScreenOrientation);
        }
    }

    private void OnEnable()
    {
        if(this.enabled && gameObject.activeSelf)
            EmulateScreenOrientation();
    }

    private void EmulateScreenOrientation()
    {
        if (Screen.width < Screen.height && lastScreenOrientation != DeviceOrientation.Portrait)
        {
            lastScreenOrientation = DeviceOrientation.Portrait;
            CnangeUI(lastScreenOrientation);
        }
        else if(Screen.width > Screen.height && lastScreenOrientation != DeviceOrientation.LandscapeLeft)
        {
            lastScreenOrientation = DeviceOrientation.LandscapeLeft;
            CnangeUI(lastScreenOrientation);
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        EmulateScreenOrientation();
    }

    private void CnangeUI(DeviceOrientation orientation)
    {
        switch (orientation)
        {
            case DeviceOrientation.Portrait:
                SnapBottom();
                break;
            
            case DeviceOrientation.LandscapeLeft:
                SnapRight();
                break;
        }
    }

    private void SnapBottom()
    {
        rect.anchorMin = new Vector2(0,0);
        rect.anchorMax = new Vector2(1,0);
        rect.pivot = new Vector2(0.5f, 0);
        rect.sizeDelta = new Vector2(0, bottomSizeY);
    }

    private void SnapRight()
    {
        rect.anchorMin = new Vector2(1,0);
        rect.anchorMax = new Vector2(1, 1f/Screen.height * (Screen.height-positionFromTop));
        rect.pivot = new Vector2(1, 0.5f);
        rect.sizeDelta = new Vector2(rightSizeX, 0);
    }
}
