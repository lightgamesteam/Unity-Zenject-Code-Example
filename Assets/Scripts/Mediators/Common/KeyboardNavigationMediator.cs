using System.Collections.Generic;
using System.Linq;
using Module.Core.UIComponent;
using TDL.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class KeyboardNavigationMediator : IInitializable, ITickable
{
    [Inject] private AsyncProcessorService _asyncProcessor;
    [Inject] private readonly SignalBus _signal;
    
    /// <summary>
    /// scene name | is generate after select ui element
    /// </summary>
    private readonly Dictionary<string, bool> workInScenes = new Dictionary<string, bool>
    {
        {SceneNameConstants.Main, true},
        {SceneNameConstants.Module3DModel, false},
        {SceneNameConstants.Module2DVideo, false},
        {SceneNameConstants.Module3DVideo, false},
        {SceneNameConstants.ModuleQuiz, false},
        {SceneNameConstants.ModulePuzzle, true},
        {SceneNameConstants.ModuleSolarSystem, false},
        {SceneNameConstants.ModuleClassification, false},
        {SceneNameConstants.ModuleDrawingTool, false},
        {SceneNameConstants.ModuleUltimate, false}
    };
    
    private List<string> loadedScenes = new List<string>();
    private bool isWork => workInScenes.ContainsKey(currentScene);

    private string currentScene
    {
        get
        {
            if(loadedScenes.Count > 0)
                return loadedScenes.Last();

            return SceneManager.GetActiveScene().name;
        }
    }

    private bool isGenerateAfterSelect
    {
        get
        {
            if (workInScenes.ContainsKey(currentScene))
                return workInScenes[currentScene];

            return false;
        }
    }

    private int currentSelectableIndex = -1;
    private Selectable currentSelectable;
    
    private List<Selectable> selectableLayer = new List<Selectable>();
    
    private List<Selectable> historySelectables = new List<Selectable>();
    
    public void SetForceSelectable(Selectable forceSelectable, bool isClear = false) 
    {
        Selectable resultSelectable = null;
        foreach (var selectable in selectableLayer) {
            if (selectable != null && selectable.gameObject.Equals(forceSelectable.gameObject)) {
                resultSelectable = selectable;
                break;
            }
        }
        if (resultSelectable != null && resultSelectable.isActiveAndEnabled) {
            SetCurrentSelectable(selectableLayer.IndexOf(resultSelectable), resultSelectable);
            if (CanSelectCurrentIndex()) {
                selectNextInfinityLoop = 0;

                Select();
                CheckInsertedLayer();
                ScrollSelectable();
            }
        } else if (isClear) {
            ClearCurrentSelectable();
        }
    }

    #region Init

    public void Initialize()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
        _signal.Subscribe<FocusKeyboardNavigationSignal>(FocusKeyboardNavigation);
    }

    private void SceneLoaded(Scene _scene, LoadSceneMode _loadSceneMode)
    {
        if (!loadedScenes.Contains(_scene.name))
            loadedScenes.Add(_scene.name);
        
        _asyncProcessor.Wait(time: 0, () =>
        {
            GenerateSelectableLayer();
            RemoveInvalidFromLayer();
        });
    }

    private void SceneUnloaded(Scene _scene)
    {
        if (loadedScenes.Contains(_scene.name))
            loadedScenes.Remove(_scene.name);
    }

    #endregion

    #region FocusKeyboardNavigationSignal
    private Dictionary<CanvasGroup, List<CanvasGroup>> canvasGroups = new Dictionary<CanvasGroup, List<CanvasGroup>>();

    private void FocusKeyboardNavigation(FocusKeyboardNavigationSignal signal)
    {
        if (signal.IsOnFocus)
        {
            if (canvasGroups.ContainsKey(signal.FocusCanvasGroup))
                canvasGroups[signal.FocusCanvasGroup].Clear();
            else
                canvasGroups.Add(signal.FocusCanvasGroup, new List<CanvasGroup>());
            
            canvasGroups[signal.FocusCanvasGroup].AddRange(GameObject.FindObjectsOfType<CanvasGroup>().ToList());

            canvasGroups[signal.FocusCanvasGroup].Remove(signal.FocusCanvasGroup);
            canvasGroups[signal.FocusCanvasGroup].RemoveAll(f => f.interactable == false || f.alpha < 0.1f || !f.gameObject.activeSelf);
            canvasGroups[signal.FocusCanvasGroup].ForEach(fo => fo.interactable = false);
        }
        else
        {
            if (canvasGroups.ContainsKey(signal.FocusCanvasGroup))
            {
                canvasGroups[signal.FocusCanvasGroup].ForEach(fo => fo.interactable = true);
                canvasGroups.Remove(signal.FocusCanvasGroup);
            }
        }
    }
    #endregion

    #region LayerOperation
    
    private void GenerateSelectableLayer(bool generateByOrder = false)
    {
        oldLayer.Clear();

        List<Selectable> generated = new List<Selectable>(FindAllSelectableOnScreen());

        if (!generated.TrueForAll(i => selectableLayer.Contains(i)))
        {
            selectableLayer = generated;
        }
        else if(generateByOrder)
        {
            selectableLayer = generated;
        }

        if (selectableLayer.Contains(currentSelectable))
        {
            currentSelectableIndex = selectableLayer.IndexOf(currentSelectable);
        }
    }
    
    private void GenerateAfterSelect()
    {
        if (currentSelectable != null)
            if (currentSelectable.gameObject.HasComponent(out SelectableTag st))
            {
                if (st.GenerateLayerOnSelect)
                {
                    GenerateSelectableLayer();
                    return;
                }
            }

        if (isGenerateAfterSelect)
            GenerateSelectableLayer();
    }
    
    private void GenerateLayerOnInteraction()
    {
        _asyncProcessor.Wait(0f, () => GenerateSelectableLayer(true));
    }

    private void Inter()
    {
        if (currentSelectable)
        {
            //if (currentSelectable.gameObject.HasComponent(out SelectableTag sel))
            var sel = currentSelectable.gameObject.GetComponents<SelectableTag>();

            if(sel.Length > 0)
            {
                foreach (SelectableTag tag in sel)
                {
                    LayerOperation(tag);
                }
            }
            else
            {
                GenerateSelectableLayer();
            }   
        }
        else
        {
            GenerateSelectableLayer();
        }
    }
    
    private void LayerOperation(SelectableTag tag)
    {
        if(tag.GenerateLayerOnInteraction)
            GenerateLayerOnInteraction();
        
        switch (tag)
        {
            case SelectableTagGenerate t:
                GenerateLayerOnInteraction();
                break;
            
            case SelectableTagSwap t:
                SwapSelectable(t);
                break;
                
            case SelectableTagGenerateInsert t:
                selectableTagGenerateInsert = t;
                InsertToLayer();
                break;
                
            case SelectableTagGenerateReplace t:
                //ReplaceLayer();
                break;
            
            case SelectableTagLayer t:
                SwapLayer(t.layer);
                break;
                
            case SelectableTagIgnore t:
                break;
        }
    }
    
    private void SwapSelectable(SelectableTagSwap selectableTagSwap)
    {
        _asyncProcessor.Wait(0.1f, () =>
        {
            currentSelectable = selectableTagSwap.swapTo;
            currentSelectable.Select();
            GenerateSelectableLayer();
        });
    }

    List<Selectable> oldLayer = new List<Selectable>();
    private void SwapLayer(List<Selectable> list)
    {
        if (oldLayer.Count == 0)
        {
            oldLayer.AddRange(selectableLayer);
            selectableLayer.Clear();
            selectableLayer.AddRange(list);
        }
        else
        {
            selectableLayer.Clear();
            selectableLayer.AddRange(oldLayer);
            oldLayer.Clear();
        }
    }
    
    private void ReplaceLayer()
    {
        List<Selectable> replaceLayer = new List<Selectable>(FindAllSelectableOnScreen());
        
        replaceLayer.RemoveAll(s => selectableLayer.Contains(s));
        replaceLayer.Add(currentSelectable);
        
        selectableLayer.Clear();
        selectableLayer.AddRange(replaceLayer);
        
        RemoveInvalidFromLayer();
        
        if(!currentSelectable.gameObject.Equals(EventSystem.current.currentSelectedGameObject))
            Select();
    }


    private SelectableTagGenerateInsert selectableTagGenerateInsert;
    private void InsertToLayer()
    {
        List<Selectable> insertList = new List<Selectable>(FindAllSelectableOnScreen());
        
        insertList.RemoveAll(s => selectableLayer.Contains(s));

        if (selectableTagGenerateInsert != null)
        {
            selectableTagGenerateInsert.layer.Clear();
            if(selectableTagGenerateInsert.isInsertMySelectable)
                selectableTagGenerateInsert.layer.Add(selectableTagGenerateInsert.mySelectable);
            selectableTagGenerateInsert.layer.AddRange(insertList);
        }
        
        selectableLayer.InsertRange(currentSelectableIndex + 1, insertList);
        RemoveInvalidFromLayer();
        
        if(!currentSelectable.gameObject.Equals(EventSystem.current.currentSelectedGameObject))
            Select();
    }
    
    private void RemoveInvalidFromLayer()
    {
        selectableLayer.RemoveAll(s => s == null || !s.isActiveAndEnabled || s.gameObject.HasComponent<SelectableTagIgnore>());
        
        if(selectableTagGenerateInsert != null)
            selectableTagGenerateInsert.layer.RemoveAll(s => s == null || !s.isActiveAndEnabled || s.gameObject.HasComponent<SelectableTagIgnore>());
    }
    
    private void RemoveInvalidFormHistoryLayer()
    {
        historySelectables.RemoveAll(s => s == null || !s.isActiveAndEnabled);
    }

    #endregion

    #region Inputs

    public void Tick()
    {
        if (isWork)
        {
            KeyboardInput();
            MouseInput();
        }
    }

    private void MouseInput()
    {
        if (Input.touchCount == 0)
        {
            if (Input.GetMouseButtonUp(0))
            {
                MouseInteraction();
            }
        }
        else
        {
            if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Ended)
            {
                MouseInteraction();
            }
        }
    }
    
    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            Interaction();
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SelectNext(-1);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SelectNext(1);
            }
        }
    }
   
    #endregion

    #region Interactions

    private void MouseInteraction()
    {
        RemoveInvalidFromLayer();

        if (selectableLayer.Count == 0)
        {
            currentSelectable = GetCurrentMouseOverSelectable();
            GenerateSelectableLayer();
        }

        GetSetCurrentSelectable();

        switch (currentSelectable)
        {
            case Button btn :
                _asyncProcessor.Wait(0, Inter);

                break;
			
            case Toggle tgl :
                _asyncProcessor.Wait(0, Inter);

                break;

            case ComponentButtonTriggers triggers:
                _asyncProcessor.Wait(0, Inter);

                break;

            case TMP_Dropdown drd:
                
                _asyncProcessor.Wait(0, InsertToLayer);
                
                break;

            case TMP_DropdownV2 drd:
                
                _asyncProcessor.Wait(0, InsertToLayer);
                
                break;
        }
    }
    
    private void Interaction()
    {
        switch (currentSelectable)
        {
            case Button btn :
                _asyncProcessor.Wait(0, Inter);

                break;
			
            case Toggle tgl :
                _asyncProcessor.Wait(0, Inter);

                break;

            case ComponentButtonTriggers triggers:
                _asyncProcessor.Wait(0, Inter);

                break;

            case TMP_Dropdown drd:
                
                if (drd.IsExpanded)
                {
                    drd.Hide();
                }
                
                _asyncProcessor.Wait(0, InsertToLayer);
                
                break;
            
            case TMP_DropdownV2 drd:
                
                if (drd.IsExpanded)
                {
                    drd.Hide();
                }
                
                _asyncProcessor.Wait(0, InsertToLayer);
                
                break;
        }
    }

    private int selectNextInfinityLoop = 0;
    private void SelectNext(int direction, bool isGetCurrentSelectable = true)
    {
        ValidateLayer();

        if(isGetCurrentSelectable)
            GetSetCurrentSelectable();
        
        SelectNextIndex(direction);

        if (CanSelectCurrentIndex())
        {
            selectNextInfinityLoop = 0;

            Select();
            CheckInsertedLayer();
            ScrollSelectable();
        }
        else
        {
            if (!isGetCurrentSelectable && selectableLayer.Count == 0) {
                ClearCurrentSelectable();
                return;
            }

            selectNextInfinityLoop++;
            if (selectNextInfinityLoop <= selectableLayer.Count)
            {
                SelectNext(direction, false);
            }
            else
            {
//                Debug.Log($"!!! INFINITY LOOP !!! [selectableLayer.Count = {selectableLayer.Count}] [selectNextInfinityLoop = {selectNextInfinityLoop}]");
                selectNextInfinityLoop = 0;
                _asyncProcessor.Wait(0, () =>
                {
                    GenerateSelectableLayer();
                    SelectNext(direction, false);
                });
            }
        }
    }

    private void ClearCurrentSelectable() 
    {
        currentSelectableIndex = -1;
        currentSelectable = null;
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    private bool CanSelectCurrentIndex()
    {
        if (selectableLayer == null || selectableLayer.Count <= currentSelectableIndex || currentSelectableIndex < 0) {
            return false;
        }

        if (selectableLayer[currentSelectableIndex] == null)
        {
            selectableLayer.RemoveAt(currentSelectableIndex);
            return false;
        }

        if (!selectableLayer[currentSelectableIndex].isActiveAndEnabled)
            return false;
        
        if(!selectableLayer[currentSelectableIndex].IsInteractable())
            return false;
        
        return true;
    }
    
    private void Select()
    {
        if (currentSelectableIndex < 0)
        {
            GenerateSelectableLayer();
            currentSelectableIndex = 0;
        }

        if (selectableLayer.Count > 0)
        {
            if (currentSelectableIndex >= 0 && currentSelectableIndex < selectableLayer.Count)
            {
                SetCurrentSelectable(currentSelectableIndex, selectableLayer[currentSelectableIndex]);
            }
        }
      
        currentSelectable.Select();
        historySelectables.Add(currentSelectable);
        GenerateAfterSelect();

#if UNITY_EDITOR
//            Debug.Log($">>>   Selected [{currentSelectableIndex}]: {selectableLayer[currentSelectableIndex].name}   <<<", selectableLayer[currentSelectableIndex].gameObject);
#endif
    }
    
    public void DeSelect()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    #endregion
    
    #region Helpers

    private void SetCurrentSelectable(int _index, Selectable _selectable)
    {
        currentSelectable = _selectable;
        currentSelectableIndex = _index;
    }

    private Selectable GetSelectableFromHistory()
    {
        RemoveInvalidFormHistoryLayer();
        
        if (historySelectables.Count > 0)
        {
            return historySelectables[historySelectables.Count - 1];
        }

        return null;
    }

    private Selectable GetCurrentMouseOverSelectable()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                if (EventSystem.current.currentSelectedGameObject.HasComponent(out Selectable selectable))
                {
                    return selectable;
                }
            }
        }

        return null;
    }

    private (int indexOnLayer, Selectable selectable) GetCurrentSelectable()
    {
        currentSelectableIndex = GetCurrentIndex();
        
        if(IsCurrentSelected())
           return (currentSelectableIndex, selectableLayer[currentSelectableIndex]);
        
        Selectable s = null;

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            foreach (Selectable sel in selectableLayer)
            {
                if (sel != null)
                {
                    if (EventSystem.current.currentSelectedGameObject.Equals(sel.gameObject))
                    {
                        s = sel;
                        break;
                    }
                }
            }
        }

        if (s == null)
        {
            s = GetSelectableFromHistory();
        }

        if (s != null)
        {
            if (s.isActiveAndEnabled)
            {
                return (selectableLayer.IndexOf(s), s);
            }

            //RemoveInvalidFromLayer();
            GenerateSelectableLayer();
        }
        
        return (-1, null);

        bool IsCurrentSelected()
        {
            if (currentSelectableIndex >= 0 && currentSelectableIndex < selectableLayer.Count && selectableLayer.Count > 0)
            {
                if(selectableLayer[currentSelectableIndex])
                    if (selectableLayer[currentSelectableIndex].gameObject
                        .Equals(EventSystem.current.currentSelectedGameObject))
                        return true;
            }
            
            return false;
        }
    }
    
    private List<Selectable> FindAllSelectableOnScreen()
    { 
        List<Selectable> sel = new List<Selectable>();
        
        if(currentScene != SceneNameConstants.Main)
            sel.AddRange(FindComponentExtension.GetAllInScene<Selectable>(SceneNameConstants.Main, false));
        
        sel.AddRange(FindComponentExtension.GetAllInScene<Selectable>(currentScene, false));
        
        sel.RemoveAll(s =>
        {
            if (s.name.Equals("Blocker") || !s.isActiveAndEnabled || s.gameObject.HasComponent<SelectableTagIgnore>())
                return true;

            if (s.GetComponentInParent<CanvasGroup>() != null && !s.GetComponentInParent<CanvasGroup>().interactable) {
                return true;
            }

            switch (s)
            {
                case Scrollbar scr:
                    return true;
            }
            
            return false;
        });

        return sel;
    }
    
    private void SelectNextIndex(int direction)
    {
        if (direction > 0)
        {
            currentSelectableIndex++;
            if (currentSelectableIndex > selectableLayer.Count - 1)
            {
                currentSelectableIndex = 0;
            }
        }
        else
        {
            currentSelectableIndex--;
            if (currentSelectableIndex < 0)
            {
                currentSelectableIndex = selectableLayer.Count - 1;
            }
        }
    }
    
    private int GetCurrentIndex()
    {
        var index = -1;

        selectableLayer.RemoveAll(item => item == null);

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Selectable currSel = selectableLayer.Find(s => s.gameObject.Equals(EventSystem.current.currentSelectedGameObject));
            if(selectableLayer.Contains(currSel))
                index = selectableLayer.IndexOf(currSel);
            
        }
        else if(currentSelectable != null)
        {
            index = selectableLayer.IndexOf(currentSelectable);
        }
        else if (historySelectables.Count > 0)
        {
            Selectable currSel = GetSelectableFromHistory();
                
            if(selectableLayer.Contains(currSel))
                index = selectableLayer.IndexOf(currSel);
        }

        return index;
    }

    private void ValidateLayer()
    {
        RemoveInvalidFromLayer();

        if (selectableLayer.Count == 0)
        {
            GenerateSelectableLayer();
        }
    }

    private void CheckInsertedLayer()
    {
        if (selectableTagGenerateInsert != null)
            if (selectableTagGenerateInsert.layer.Count > 0 && selectableTagGenerateInsert.isInvokeOnOutLayer)
            {
                if (!selectableTagGenerateInsert.layer.Contains(currentSelectable))
                {
                    selectableTagGenerateInsert.onOutLayerEvent?.Invoke();
                    selectableTagGenerateInsert = null;
                    GenerateSelectableLayer();
                }
            }
    }

    private void ScrollSelectable()
    {
        var scrollable = currentSelectable.gameObject.GetComponent<SelectableTagScrollable>();
        if (scrollable != null)
        {
            scrollable.ScrollRectToSelection(currentSelectable.GetComponent<RectTransform>());
        }
    }

    private void GetSetCurrentSelectable()
    {
        (int indexOnLayer, Selectable selectable) sel = GetCurrentSelectable();

        SetCurrentSelectable(sel.indexOnLayer, sel.selectable);
    }

    private void DestroyLayer()
    {
        selectableLayer.Clear();
        currentSelectableIndex = -1;
    }

    #endregion
}