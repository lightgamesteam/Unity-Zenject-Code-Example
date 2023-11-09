using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Module.Core.Attributes;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if !UNITY_WEBGL
using Vuforia;
#endif
using Image = UnityEngine.UI.Image;
using Renderer = UnityEngine.Renderer;

public class LabelData : ViewBase
{
    //[InjectOptional] private readonly SignalBus _signal;
    [InjectOptional] private AccessibilityModel _accessibilityModel;

    [ShowOnly] public int ID;
    public string LabelId { get; set; }
    public bool IsMultiViewSecond { get; set; }

    [Inject] public string labelText;
    [SerializeField] public string modelPartName;
    [Inject] public Color headerColor;
    
    [SerializeField] protected TextMeshProUGUI headerLabel;
    [SerializeField] protected Image headerImage;
    [SerializeField] protected Button hotSpotButton;
    [SerializeField] protected EventTrigger eventTrigger;
    
    // Description
    [SerializeField] private Button _descriptionButton;
    public Button DescriptionButton => _descriptionButton;
    public LabelLocalName[] LabelLocalNames { get; set; }

    private bool isHotSpotButtonActive;
    public static Action<bool> SetActiveHotSpotAction = delegate {};
    
    //-----------------------
    private Renderer headerRenderer;
    private ClickTextEvent clickTextEvent;
    private IEnumerator waitTTS;

    public override void InitUiComponents()
    {
        SetActiveHotSpotAction += SetActiveHotSpot;
        headerLabel.fontSize /= _accessibilityModel.ModulesFontSizeScaler;

        eventTrigger.triggers.ToList().ForEach(ev =>
        {
            switch (ev.eventID)
            {
                case EventTriggerType.PointerEnter:
                    ev.callback.AddListener(t =>
                    {
                        if (Input.touchCount == 0)
                        {
                            waitTTS = WaitTTS();
                            StartCoroutine(waitTTS);
                        }

                        EnableModelPartOutline(true);
                    });
                    break;

                case EventTriggerType.PointerExit:
                    ev.callback.AddListener(t =>
                    {
                        if (Input.touchCount == 0)
                        {
                            if (waitTTS != null)
                                StopCoroutine(waitTTS);
                        }

                        EnableModelPartOutline(false);
                    });
                    break;

                case EventTriggerType.PointerClick:
                    ev.callback.AddListener(t =>
                    {
                        if (Input.touchCount > 0)
                        {
                            waitTTS = WaitTTS();
                            StartCoroutine(waitTTS);
                        }
                    });
                    break;
            }
        });
    }

    public override void SubscribeOnListeners()
    {
        SetData(labelText, headerColor);
        GetCurrentCamera();
        
        if (clickTextEvent == null)
        {
            gameObject.HasComponent(out clickTextEvent);
        }
    }

    private void OnDestroy()
    {
        SetActiveHotSpotAction -= SetActiveHotSpot;
    }

    private IEnumerator WaitTTS()
    {
        yield return new WaitForSeconds(1f);
        clickTextEvent.Invoke();
    }
    
    public void SetHotSpotButtonActive(bool isActive)
    {
        isHotSpotButtonActive = isActive;
        hotSpotButton.gameObject.SetActive(isActive);
    }

    public void SetActiveHotSpot(bool isActive)
    {
        if (isActive)
        {
            hotSpotButton.gameObject.SetActive(isHotSpotButtonActive);
        }
        else
        {
            hotSpotButton.gameObject.SetActive(false);
        }
    }
    
    public Button GetHotSpotButton()
    {
        return hotSpotButton;
    }

    #region Description

    public void SetDescriptionVisibility(bool status)
    {
        DescriptionButton.gameObject.SetActive(status);
    }

    #endregion

    public Vector3[] GetHeaderCorners()
    {
        Vector3[] corners = new Vector3[4];
        headerImage.rectTransform.GetWorldCorners(corners);
        return corners;
    }

    public void SetModelPartName(string partName)
    {
        modelPartName = partName;
        gameObject.name = partName;
    }

    public virtual void SetData(string text, Color32 _headerColor)
    {
        labelText = text;
        headerColor = _headerColor;

        headerLabel.text = labelText;
        headerLabel.enableCulling = true;
        headerImage.color = headerColor;
        
        //-----------------
        
        //headerRenderer = backgroundHeader.gameObject.GetComponent<Renderer>();
        //headerRenderer.material.color = headerColor;
        
        //textMeshPro.text = labelText;
        //textMeshPro.enableCulling = true;
        //textMeshPro.ForceMeshUpdate();
        
       // UpdateLabelObjectBox();
    }
    
    public void EnableHighlight(bool isEnabled)
    {
//         if (textMeshPro == null)
//         {
// #if UI_PC
//             _signal.Fire(new SendCrashAnalyticsCommandSignal("!!! Enable Label Highlight > Missing: textMeshPro"));
// #endif
//             return;
//         }
        
        headerLabel.fontStyle = isEnabled ? FontStyles.Underline : FontStyles.Normal;
        //textMeshPro.fontStyle = isEnabled ? FontStyles.Underline : FontStyles.Normal;
    }

    public virtual void SetText(string text)
    {
        labelText = text;
        headerLabel.text = labelText;

        //textMeshPro.text = labelText;
        //textMeshPro.ForceMeshUpdate();
        //UpdateLabelObjectBox();
    }

    public virtual void SetDescriptionIconState(bool status)
    {
        DescriptionButton.gameObject.SetActive(status);
    }
    
    // public virtual void SetLabelScale(float fontSizeScaleFactor)
    // {
    //     if(fontSizeScaleFactor <= 0 || fontSizeScaleFactor == 1)
    //         return;
    //     
    //     textMeshPro.fontSize /= fontSizeScaleFactor;
    //     textMeshPro.ForceMeshUpdate();
    //     UpdateLabelObjectBox();
    // }
    //
    // private void UpdateLabelObjectBox()
    // {
    //     var textSize = textMeshPro.textBounds;
    //     var bodySize = textSize.extents * LabelScaleMultiplier + new Vector3(0f, 0f, 1f);
    //     
    //     if (backgroundBody != null)
    //     {
    //         backgroundBody.transform.localScale =  new Vector3(
    //             bodySize.x,
    //             backgroundBody.transform.localScale.y,
    //             backgroundBody.transform.localScale.z);
    //     }
    //
    //     if (backgroundHeader != null)
    //     {
    //         backgroundHeader.transform.localScale = new Vector3(
    //             bodySize.x,
    //             backgroundHeader.transform.localScale.y,
    //             backgroundHeader.transform.localScale.z);
    //     }
    // }
    //
    // private void OnMouseDown()
    // {
    //     if (EventSystem.current.IsPointerOverGameObject())
    //         return;
    //     
    //     if (Input.touchCount > 0)
    //     {
    //         if (Input.touchCount == 1)
    //         {
    //             EnableOutline(true);
    //             isMouseEnter = true;
    //         }
    //     }
    // }
    //
    // private void OnMouseUpAsButton()
    // {
    //     if (EventSystem.current.IsPointerOverGameObject())
    //         return;
    //     
    //     if(waitTTS != null)
    //         StopCoroutine(waitTTS);
    //     
    //     if (Input.touchCount > 0)
    //     {
    //         if (Input.touchCount == 1)
    //         {
    //             EnableOutline(false);
    //             isMouseEnter = false;
    //         }
    //     }
    // }
    //
    // public void OnMouseEnter()
    // {
    //     if (EventSystem.current.IsPointerOverGameObject() || Input.touchCount > 0)
    //         return;
    //
    //     if (!isMouseEnter)
    //     {
    //         waitTTS = WaitTTS();
    //         StartCoroutine(waitTTS);
    //         EnableOutline(true);
    //         isMouseEnter = true;
    //     }
    // }
    //
    // private void OnMouseExit()
    // {
    //     if (Input.touchCount > 0)
    //         return;
    //
    //     if (isMouseEnter)
    //         isMouseEnter = false;
    //     
    //     if(waitTTS != null)
    //         StopCoroutine(waitTTS);
    //     EnableOutline(false);
    // }

    private Camera cam;
    protected void GetCurrentCamera()
    {
#if !UNITY_WEBGL
        if (VuforiaBehaviour.Instance == null)
        {
            cam = gameObject.GetInSceneOnLayer<Camera>();
        }
        else
        {
            cam = VuforiaBehaviour.Instance.gameObject.GetComponent<Camera>();
        }
#else
        cam = gameObject.GetInSceneOnLayer<Camera>();
#endif
        gameObject.GetComponentInChildren<Canvas>().worldCamera = cam;
    }
    
    public bool IsCameraLookOnLabelFace(float value = 0f)
    {
        if (!cam)
            return false;
        
        float dot = Vector3.Dot(transform.forward, cam.transform.forward);
        
        return dot > value;
    }

    private void EnableModelPartOutline(bool value)
    {
        bool active = value && IsCameraLookOnLabelFace();

        List<ObjectHighlighter> objects = FindComponentExtension.GetAllInSceneOnLayer<ObjectHighlighter>(SceneNameConstants.Module3DModel, gameObject.layer);

        ObjectHighlighter oh = objects.Find(o => o.transform.name == modelPartName && o.ID == ID);
        
        oh?.SetOutline(active);
    }
    
    public class Factory : PlaceholderFactory<string, Color, LabelData>
    {

    }
}
