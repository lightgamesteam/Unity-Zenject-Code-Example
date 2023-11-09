using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Linq;
using Module.IK;
using TDL.Constants;
using TDL.Models;

namespace TDL.Views
{
    public class LabelLine : MonoBehaviour
    {
        [SerializeField] protected LabelData labelData;
        [SerializeField] protected LineRenderer lineRenderer;
        [SerializeField] protected EventTrigger eventTrigger;

        private bool isOnLabelLine;

        private void Awake()
        {
            lineRenderer.material.color = labelData.headerColor;
            isOnLabelLine = PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.AccessibilityLabelLines) && !PlayerPrefsExtension.GetBool(PlayerPrefsKeyConstants.ARmodeSettings);

            if (isOnLabelLine)
            {
                RelocatorView.IsRelocatorActivatedAction += UpdateLabelLine;
            }
            
            SetLabelLineRendererActive(isOnLabelLine);
            SetLabelLineGameObjectActive(false);
            
            SetLabelLineXrayActive(false);

            if (eventTrigger != null)
            {
                eventTrigger.triggers.ToList().ForEach(ev =>
                {
                    switch (ev.eventID)
                    {
                        case EventTriggerType.PointerEnter:
                            ev.callback.AddListener(t =>
                            {
                                if (Input.touchCount == 0)
                                {
                                    SetLabelLineXrayActive(true);
                                }
                            });
                            break;
                
                        case EventTriggerType.PointerExit:
                            ev.callback.AddListener(t =>
                            {
                                if (Input.touchCount == 0)
                                {
                                    SetLabelLineXrayActive(false);
                                }
                            });
                            break;
                    }
                });   
            }
        }

        private void OnDestroy()
        {
            if (isOnLabelLine)
            {
                RelocatorView.IsRelocatorActivatedAction -= UpdateLabelLine;
            }
        }

        private void UpdateLabelLine(bool IsRelocatorActivated, int layer)
        {
            if(layer != gameObject.layer || !enabled || !gameObject.activeSelf)
                return;
            
            if (IsRelocatorActivated)
            {
                SetLabelLineGameObjectActive(false);
            }
            else
            {
                AsyncProcessorService.Instance.Wait(0, () =>
                {
                    CalculateLabelLinePosition();
                    SetLabelLineGameObjectActive(true);
                });
            }
        }

        private void OnEnable()
        {
            ContentViewModel.OnChangeLayerLanguage += OnChangeLayerLanguage;
            
            List<ObjectHighlighter> objects =  FindComponentExtension.GetAllInScene<ObjectHighlighter>(gameObject.scene.name);
            myObjectHighlighter = objects.Find(o => o.transform.name == labelData.modelPartName && o.ID == labelData.ID);
            
            StartCoroutine(StartCalculateLabelLinePosition());

            if (isOnLabelLine == false)
            {
                SetLabelLineGameObjectActive(false);
            }
        }

        private void OnDisable()
        {
            ContentViewModel.OnChangeLayerLanguage -= OnChangeLayerLanguage;
        }
        
        private void OnChangeLayerLanguage((int layer, string cultureCode) changeLanguage)
        {
            if (changeLanguage.layer == gameObject.layer)
                StartCoroutine(StartCalculateLabelLinePosition());
        }

        private IEnumerator StartCalculateLabelLinePosition()
        {
            yield return new WaitForEndOfFrame();
            CalculateLabelLinePosition();
        }

        // [SerializeField] private bool isMouseEnter = false;
        // public void OnMouseEnter()
        // {
        //     if (EventSystem.current.IsPointerOverGameObject() || Input.touchCount > 0)
        //         return;
        //
        //     if (!isMouseEnter)
        //     {
        //         SetLabelLineXrayActive(true);
        //
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
        //     SetLabelLineXrayActive(false);
        // }

        public void SetLabelLineXrayActive(bool value)
        {
            SetLabelLineXrayValue(value ? 8 : 2);
        }

        public bool IsActiveLineRender;
        public void SetLabelLineGameObjectActive(bool value, bool overrideActive = true)
        {
            if (overrideActive)
                IsActiveLineRender = value;
                
            if(value && lineRenderer.enabled && !isCalculatedLabelLinePosition)
                CalculateLabelLinePosition();
            
            lineRenderer.gameObject.SetActive(value);
        }
        
        private void SetLabelLineRendererActive(bool value)
        {
            lineRenderer.enabled = value;
        }
        
        private void SetLabelLineXrayValue(int i)
        {
            lineRenderer.material.SetFloat("_ZTestXRay", i);
        }

        private bool isCalculatedLabelLinePosition;
        private ObjectHighlighter myObjectHighlighter;
        private void CalculateLabelLinePosition()
        {
            // Second Point
            //Vector3 sP = oh.gameObject.GetAllVertexPosition(true).GetClosestPositionTo(MouseExtension.GetDepthCameraForMousePosition().transform.position);
            //Vector3 sP = oh.gameObject.GetAllVertexPosition(true).GetClosestPositionTo(oh.gameObject.GetBounds(true).center);
            Vector3 sP = Vector3.zero;

            if (myObjectHighlighter)
            {
                if (myObjectHighlighter.gameObject.HasComponent(out SkinnedMeshRenderer osmr))
                {
                    // - Disable -
                    //DisableLabelLine();

                    osmr.BakeAndGetAllVertexPosition(out var osmrVertices);
                    sP = osmrVertices.GetClosestPositionTo(transform.position);
                }
                else
                {
                    sP = myObjectHighlighter.gameObject.GetAllVertexPosition(true).GetClosestPositionTo(transform.position);
                }
            }
            else
                return;

            lineRenderer.SetPosition(1, sP);

            // First Point
            Vector3 fP = labelData.GetHeaderCorners().GetClosestPositionTo(sP);
            
            lineRenderer.SetPosition(0, fP);
            isCalculatedLabelLinePosition = true;
        }

        private void Update()
        {
            if (IsActiveLineRender && isOnLabelLine)
            {
                SetLabelLineGameObjectActive(labelData.IsCameraLookOnLabelFace(0.2f), false);
            }
        }
    }
}