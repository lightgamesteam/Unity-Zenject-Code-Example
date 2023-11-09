using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MultiView : ViewBase
{
    [Inject] public int renderLayerIndex;
    [Inject] public MultiViewType multiViewType;

    public RawImage BackgroundRawImage;

    public Image Frame;
    public GameObject RenderLayer;
    public RenderView MultimodelView;
    public RenderView MultipartView;
    
    public override void InitUiComponents()
    {
        gameObject.transform.SetLayer(renderLayerIndex);

        switch (multiViewType)
        {
            case MultiViewType.Main:
                break;

            case MultiViewType.Multipart:
                MultimodelView.gameObject.SetActive(false);
                MultipartView.gameObject.SetActive(true);
                MultipartView.RenderCamera.cullingMask = 1 << renderLayerIndex;
                MultipartView.Light.cullingMask = 1 << renderLayerIndex;
                MultipartView.MultiView = this;
                break;

            case MultiViewType.Multimodel:
                MultipartView.gameObject.SetActive(false);
                MultimodelView.gameObject.SetActive(true);
                MultimodelView.RenderCamera.cullingMask = 1 << renderLayerIndex;
                MultimodelView.Light.cullingMask = 1 << renderLayerIndex;
                MultimodelView.MultiView = this;
                break;
        }
    }

    public void ScaleFontSize(float scale)
    {
        if(scale <= 0 || scale == 1f)
            return;
        
        gameObject.GetAllComponentsInChildren<Text>().ForEach(t => t.fontSize = (int) (t.fontSize / scale));
        gameObject.GetAllComponentsInChildren<TextMeshProUGUI>().ForEach(t => t.fontSize /= scale);
    }

    public class Factory : PlaceholderFactory<int, MultiViewType, MultiView>
    {

    }
}

public enum MultiViewType
{
    Main,
    Multimodel,
    Multipart
}