using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDL.Views
{
    public class LabelDataMobile : LabelData
    {
        public static Action UpdateMobileLabelAction = delegate {  };
        public TextMeshProUGUI myToggleText;

        private void UpdateMobileLabel()
        {
            if(myToggleText == null)
                return;

            string header = myToggleText.text.Split('.')[0];
            SetText(header);
        }
        
        public override void InitUiComponents()
        {
            UpdateMobileLabelAction += UpdateMobileLabel;
        }

        private void OnDestroy()
        {
            UpdateMobileLabelAction -= UpdateMobileLabel;
        }

        public override void SubscribeOnListeners()
        {
            SetData(labelText, headerColor);
            GetCurrentCamera();
        }

        public override void SetData(string text, Color32 headerColor)
        {
            labelText = text;
            this.headerColor = headerColor;
            headerImage.color = this.headerColor;

            headerLabel.text = labelText;
            headerLabel.enableCulling = true;
        }

        public override void SetText(string text)
        {
            labelText = text;
            headerLabel.text = text;
        }
            
        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            if (Input.touchCount > 0)
            {
                if (Input.touchCount == 1)
                {
                    clickMousePosition = Input.GetTouch(0).position;
                }
            }
            else
            {
                clickMousePosition = Input.mousePosition;
            }
        }
        
        private Vector2 clickMousePosition;
        private float mouseClickDistance = 7f;
        private void OnMouseUpAsButton()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.touchCount > 0)
            {
                if (Input.touchCount == 1)
                {
                    if (Vector2.Distance(clickMousePosition, Input.GetTouch(0).position) <= mouseClickDistance)
                    {
                        EnableOutline();
                    }
                }
            }
            else
            {
                if (Vector2.Distance(clickMousePosition, Input.mousePosition) <= mouseClickDistance && Application.isEditor)
                {
                    EnableOutline();
                }
            }
        }
        
        public void EnableOutline()
        {
            if(!IsCameraLookOnLabelFace())
                return;
        
            List<ObjectHighlighter> objects = gameObject.GetAllInScene<ObjectHighlighter>();

            ObjectHighlighter oh = objects.Find(o => o.transform.name == modelPartName && o.ID == ID);

            bool isEnabled = oh.OutlineIsEnabled();
        
            foreach (ObjectHighlighter o in objects)
            {
                o.SetOutline(false);
            }
        
            oh?.SetOutline(!isEnabled);
            oh?.GetMyToggle().SetValue(true, true);
        }
    
        public void OnMouseEnter()
        {
        
        }

        private void OnMouseExit()
        {
            
        }
    }
}