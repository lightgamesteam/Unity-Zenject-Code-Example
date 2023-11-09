using System;
using System.Collections.Generic;
using System.Linq;
using Module.IK;
using TDL.Core;
using TDL.Modules.Model3D;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XDPaint;
using XDPaint.Tools;
using Zenject;

public class ContentPaint3DMediator : IInitializable, IDisposable, ITickable
{
    [Inject] private GameInstaller.UIElementPrefabs _screenContainer;
    [Inject] private SignalBus _signal;

    private PaintView _view;
    private GameObject _paintCanvas3D;
    private SmoothOrbitCam _smoothOrbitCam => _view.smoothOrbitCam;
    private Camera _camera => _view.camera;
    
    private Transform _paintContainer;
    private PaintManager3DL _currPaintManager;

    private Canvas _currTextCanvas;
    private TMP_InputField _lastSelectedInputField;
    private List<ContentPaint3DMediator> _mediators = new List<ContentPaint3DMediator>();
    
    public void Initialize()
    {
       _signal.Subscribe<InitializePaintSignal>(InitializeSignal);
       PaintManager3DL.OnDestroyManager += OnDestroyManager;
    }
    
    private void InitializeSignal(InitializePaintSignal signal)
    {
        foreach (ContentPaint3DMediator mediator in _mediators)
        {
            if(mediator._view == signal.PaintView)
                return;
        }

        ContentPaint3DMediator cpm = new ContentPaint3DMediator
        {
            _view = signal.PaintView, _paintCanvas3D = _screenContainer.PaintCanvas3D, _signal = _signal
        };
        
        cpm.InitializeModule();
        _mediators.Add(cpm);
        DisposeMediators();
    }

    private void OnDestroyManager()
    {
        _mediators.ForEach(m =>
        {
            m.TurnOffTextEdit();

            // if (m._view != null)
            // {
            //     m.DeInitPaint();
            // }
        });
        
        AsyncProcessorService.Instance.Wait(0, DisposeMediators);
    }

    private void DisposeMediators()
    {
        _mediators.ForEach(m =>
        {
            if (m._view == null)
            {
                m.Dispose();
            }
        });

        _mediators.RemoveAll(m => m == null);
    }

    protected void InitializeModule()
    {
        ContentViewMediatorBase.InitializeModuleAction += ClearAllOnLoad;
        _signal.Subscribe<ContentPaint3DStateChangedSignal>(ContentPaint3DStateChanged);

        _view.paint3DToggle.onValueChanged.AddListener(PaintStateChanged);
        _view.paintEraseToggle.onValueChanged.AddListener(EraseStageChange);
        _view.paint3DClearAllButton.onClick.AddListener(ClearAll);
        _view.paint3DTextToggle.onValueChanged.AddListener(TextStageChange);
        _view.paint3DAllTextToggle.onValueChanged.AddListener(EditAllTextStageChange);
        _view.paint3DBrushColorPiker.gameObject.SetActive(false);
        _paintContainer = _view.gameObject.GetInScene<ControllersContainer>("[XDPaintContainer]").transform;
        
        if (_view.paintViewType == PaintViewType.VideoView)
        {
            _view.camera = MonoBehaviour.Instantiate(_camera, _view.camera.transform, true);

            _view.camera.depth += 1;
            _paintContainer.gameObject.layer = 12;
            _view.camera.cullingMask = LayerMask.GetMask("RenderLayer3");
        }
    }

    private void ContentPaint3DStateChanged(ContentPaint3DStateChangedSignal signal)
    {
        _view.paint3DToggle.isOn = signal.IsOnState;
        
        if (signal.IsClearAll)
            ClearAll();
    }

    public void Tick()
    {
        _mediators.ForEach(m =>
        {
            m.TextCreateTick();
        });
    }

    #region Erase

    private void EraseStageChange(bool isOn)
    {
        _currPaintManager.SetEraseState(isOn);
    }

    private void ClearAllOnLoad(int id)
    {
        ClearAll();    
    }
    
    private void ClearAll()
    {
        if(_paintContainer == null || _currPaintManager == null)
            return;
            
        _paintContainer.gameObject.GetAllComponentsInChildren<PaintManager3DL>().ForEach(pm =>
        {
            if(pm != _currPaintManager)
                pm.gameObject.SelfDestroy();
        });
        
        _currPaintManager.ClearPaintTexture();
        
        _paintContainer.gameObject.GetAllComponentsInChildren<PaintTextInputItem>().ForEach(inputField =>
        {
            if(inputField != _currPaintManager.textInputTemplate)
                inputField.gameObject.SelfDestroy();
        });
    }

    #endregion

    #region Text

    private bool isOnTextEdit = false;
    private void TurnOffTextEdit()
    {
        isOnTextEdit = false;
    }

    private void EditAllTextStageChange(bool isOn)
    {
        _paintContainer.gameObject.GetAllComponentsInChildren<PaintTextInputItem>().ForEach(p =>
        {
            if (p.myPaintManager != _currPaintManager)
            {
                p.SetInteractable(isOn && p.IsVisible());
            }
        });
    }

    private void TextStageChange(bool isOn)
    {
        isOnTextEdit = isOn;
        _view.paint3DAllTextToggle.gameObject.SetActive(isOn);
        EditAllTextStageChange(_view.paint3DAllTextToggle.isOn && isOn);

        if (_currTextInputItem)
        {
            if(!isOn)
                _currTextInputItem.ShowTopPanel(false);
        }

        if(isOn)
            _currPaintManager.DeselectAllCanvasForPainting();
        else
            _currPaintManager.SelectCanvasForPainting();
        
        _currPaintManager.gameObject.GetAllComponentsInChildren<PaintTextInputItem>().ForEach(p => p.SetInteractable(isOn));
    }

    protected void TextCreateTick()
    {
        if(!isOnTextEdit || EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonUp(0))
        {
            CreateText();
        }
    }

    private void CreateText()
    {
        PaintTextInputItem instance = MonoBehaviour.Instantiate(_currPaintManager.textInputTemplate,  _currPaintManager.textInputTemplate.transform.parent, false);
        instance.gameObject.SetActive(true);
        instance.transform.position = GetPosition();
        instance.SetTextColor(_view.paint3DBrushColorPiker.CurrentColor);
        instance.myPaintManager = _currPaintManager;
        
        instance.moveEventTrigger.triggers.ToList().ForEach(ev =>
        {
            switch (ev.eventID)
            {
                case EventTriggerType.Drag:
                    ev.callback.AddListener(t => MoveText(instance.transform));
                    break;
                
                case EventTriggerType.BeginDrag:
                    ev.callback.AddListener(t => instance.isOnDrag = true);
                    break;
                
                case EventTriggerType.EndDrag:
                    ev.callback.AddListener(t =>instance.isOnDrag = false);
                    break;
            }
        });
       
        instance.ShowTopPanel(false);
        instance.inputField.onSelect.AddListener(s => OpenTextTopPanel(instance));
        instance.inputField.onDeselect.AddListener(s => { CloseTextTopPanel(instance); });
    }

    private PaintTextInputItem _currTextInputItem;
    private void OpenTextTopPanel(PaintTextInputItem instance)
    {
        if (_currTextInputItem)
        {
            _currTextInputItem.ShowTopPanel(false);
        }

        instance.ShowTopPanel(true);
        _currTextInputItem = instance;
    }
    
    private void CloseTextTopPanel(PaintTextInputItem instance)
    {
        if (!AsyncProcessorService.Instance.ApplicationInFocus)
            return;

        if (Input.mousePresent && Input.touchCount == 0)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
        }

        if (Input.touchSupported)
        {
            foreach (var touch in Input.touches)
            {
                int id = touch.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(id))
                    return;
            }
        }

        AsyncProcessorService.Instance.Wait(0.5f, () =>
        {
            if (instance != null && !instance.isOnDrag)
                instance.ShowTopPanel(false);
        });
    }

    private void MoveText(Transform panel)
    {
        panel.position = GetPosition();
    }
    
    private Vector3 GetPosition()
    {
         RectTransformUtility.ScreenPointToLocalPointInRectangle(
             _currTextCanvas.transform as RectTransform,
            Input.mousePosition, _currTextCanvas.worldCamera,
            out Vector2 movePos);
         return _currTextCanvas.transform.TransformPoint(movePos);
    }
    
    #endregion

    #region Paint
    private void PaintStateChanged(bool isOn)
    {
        if (_view.paintViewType == PaintViewType.ContentView)
        {
            ContentView();
        }
        else if (_view.paintViewType == PaintViewType.VideoView)
        {
            VideoView();
        }

        void VideoView()
        {
            if (isOn)
            {
                InitPaintVideoView();
            }
            else
            {
                DeInitPaint();
            }
        }

        void ContentView()
        {
            _smoothOrbitCam.interactable = !isOn;
        
            _view.gameObject.GetAllInScene<RelocatorView>().ForEach(rel =>
            {
                rel.isInteractable = !isOn;
            });

            _view.gameObject.GetAllInScene<ObjectHighlighter>().ForEach(oh =>
            {
                oh.interactable = !isOn;
            });

            if (isOn)
            {
                InitPaint();
            }
            else
            {
                DeInitPaint();
            }
        }
    }
    
    void InitPaintVideoView()
    {
        if (_currPaintManager)
        {
            _view.paint3DBrushSizeSlider.onValueChanged.RemoveAllListeners();
            _view.paint3DBrushColorPiker.onValueChanged.RemoveAllListeners();
        }
        
        GameObject paintCanvas = MonoBehaviour.Instantiate(_paintCanvas3D, _paintContainer, true);
        _currPaintManager = paintCanvas.GetComponent<PaintManager3DL>();
        
        _currPaintManager.transform.SetLayer(12);
        _currPaintManager.GetComponent<MeshRenderer>().sortingOrder = 100;
        
        // _currPaintManager.SetBrushSize(_view.paint3DBrushSizeSlider.value);
        // _currPaintManager.SetBrushColor(_view.paint3DBrushColorPiker.CurrentColor);

        // -------------------
        _currPaintManager.transform.position = _camera.transform.forward * 6f;
        _currPaintManager.transform.rotation = Quaternion.LookRotation(_currPaintManager.transform.position - _camera.transform.position);
        float anglePM = Vector3.Angle(_camera.transform.up, _currPaintManager.transform.up);

        if (anglePM > 0)
            _currPaintManager.transform.Rotate(new Vector3(0, 0, 1), anglePM);
        
        // -------------------
        InitPaintManager(true);
        AddTextureToMemoryManager();
    }

    private void InitPaintManager(bool changeSortingOrder = false)
    {
        _currPaintManager.Initialize();
        _currTextCanvas = _currPaintManager.gameObject.GetFirstInChildren<Canvas>();
        _currTextCanvas.worldCamera = _camera;
        _currPaintManager.Camera = _camera;
        
        if(changeSortingOrder)
            _currTextCanvas.sortingOrder = 101;

        _currPaintManager.SetBrushSize(_view.paint3DBrushSizeSlider.value);
        _currPaintManager.SetBrushColor(_view.paint3DBrushColorPiker.CurrentColor);
        _view.paint3DBrushSizeSlider.onValueChanged.AddListener(_currPaintManager.SetBrushSize);
        _view.paint3DBrushColorPiker.onValueChanged.AddListener((c) =>
        {
            _currPaintManager.SetBrushColor(c);
            PaintTextInputItem.SetCurrTextColor?.Invoke(c);
        });
    }

    private void AddTextureToMemoryManager()
    {
        _signal.Fire(new AddObjectToMemoryManagerSignal(_view.gameObject.scene.name,
            _currPaintManager.GetRenderTextureHelper().PaintTexture));
        _signal.Fire(new AddObjectToMemoryManagerSignal(_view.gameObject.scene.name,
            _currPaintManager.GetRenderTextureHelper().CombinedTexture));
        _signal.Fire(new AddObjectToMemoryManagerSignal(_view.gameObject.scene.name,
            _currPaintManager.GetMaterialsContainer().SourceTexture));
    }

    void InitPaint()
    {
        if (_currPaintManager)
        {
            _view.paint3DBrushSizeSlider.onValueChanged.RemoveAllListeners();
            _view.paint3DBrushColorPiker.onValueChanged.RemoveAllListeners();
        }
        
        GameObject paintCanvas = MonoBehaviour.Instantiate(_paintCanvas3D, _paintContainer, true);
        _currPaintManager = paintCanvas.GetComponent<PaintManager3DL>();
        
        // _currPaintManager.SetBrushSize(_view.paint3DBrushSizeSlider.value);
        // _currPaintManager.SetBrushColor(_view.paint3DBrushColorPiker.CurrentColor);
        
        SetPosCurrPaintManagerTask(() =>
        {
            InitPaintManager();
            AddTextureToMemoryManager();
        });
    }
    
    void SetPosCurrPaintManagerTask(Action callBack)
    {
        //GameObject model = _view.model.transform.Find("model").gameObject;
        var camPosition = _camera.transform.position;
        var camForward = _camera.transform.forward;

        if(_view.model == null)
        {
            _view.model = _view.gameObject.GetInScene<Transform>("model").gameObject;
        }
        
        _view.model.GetAllVertexPosition(true, r =>
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            r.GetClosestPositionTo(camPosition, Calc);
#else
            r.TaskGetClosestPositionTo(camPosition, Calc);
#endif
        });

        void Calc(Vector3 closestToCamera)
        {
            float dis = 0f;
            float angleCam = Vector3.Angle(camForward, (closestToCamera - camPosition).normalized) * Mathf.Deg2Rad;
            float disCam = Vector3.Distance(camPosition, closestToCamera) - 0.25f;

            dis = disCam * Mathf.Cos(angleCam);

            _currPaintManager.transform.position = camPosition + camForward * dis;
            _currPaintManager.transform.rotation =
                Quaternion.LookRotation(_currPaintManager.transform.position - camPosition);

            float anglePM = Vector3.Angle(_camera.transform.up, _currPaintManager.transform.up);

            if (anglePM > 0)
                _currPaintManager.transform.Rotate(new Vector3(0, 0, 1), anglePM);
            
            callBack.Invoke();
        }
    }
    
    void DeInitPaint()
    {
        _view.paint3DBrushSizeSlider.transform.parent.parent.parent.gameObject.GetComponentsInChildren<Toggle>(true).ToList().ForEach(t => t.isOn = false);
        _currPaintManager?.DeselectAllCanvasForPainting();

        Debug.Log($"Paint Log. {nameof(DeInitPaint)}");
    }
#endregion

    public void Dispose()
    {
        ContentViewMediatorBase.InitializeModuleAction -= ClearAllOnLoad;
        _signal.TryUnsubscribe<ContentPaint3DStateChangedSignal>(ContentPaint3DStateChanged);
        GC.SuppressFinalize(this);
    }
}
