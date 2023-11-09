using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;

public class PaintView  : ViewBase
{
    public PaintViewType paintViewType = PaintViewType.ContentView;
        
    public SmoothOrbitCam smoothOrbitCam;
    public Camera camera;
    public GameObject model;
    
    public Toggle paint3DToggle;
    public Slider paint3DBrushSizeSlider;
    public ColorPickerControl paint3DBrushColorPiker;
    public Toggle paintEraseToggle;
    public Toggle paint3DTextToggle;
    public Toggle paint3DAllTextToggle;
    public Button paint3DClearAllButton;

    public override void InitUiComponents()
    {
        paint3DToggle = transform.Get<Toggle>("Toggle_Paint3D");
        paint3DBrushSizeSlider = transform.Get<Slider>("X_Panel_Paint3D/List_Container/Toggle_BrushSize/X_Panel_BrushSize/BrushSize_Slider");
        paint3DBrushColorPiker = transform.Get<ColorPickerControl>("X_Panel_Paint3D/List_Container/X_ColorPicker 2.0");
        paintEraseToggle = transform.Get<Toggle>("X_Panel_Paint3D/List_Container/Toggle_Erase");
        paint3DTextToggle = transform.Get<Toggle>("X_Panel_Paint3D/List_Container/Toggle_Text");
        paint3DAllTextToggle = transform.Get<Toggle>("X_Panel_Paint3D/List_Container/X_Toggle_AllText");
        paint3DClearAllButton = transform.Get<Button>("X_Panel_Paint3D/List_Container/Button_ClearAll");
    }
}

public enum PaintViewType
{
    ContentView,
    VideoView
}
