using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace Module.IK
{

    public class UIController
    {

        public static void TogglePanelUI()
        {
            if (!DataModel.IsOpenUIPanel)
            {
                OpenPanelUI();
            }
            else
            {
                ClosePanelUI();
            }
        }


        public static bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            var isUI = results.Count > 0;
            if (isUI && results[0].gameObject.layer.Equals(2))
            {
                isUI = !isUI;
            }

            return isUI;
        }



        private static void OpenPanelUI()
        {
            if (!DataModel.IsOpenUIPanel)
            {
                MoveUIPanel(false);
            }
        }


        private static void ClosePanelUI()
        {
            if (DataModel.IsOpenUIPanel)
            {
                MoveUIPanel(true);
            }
        }


        private static void MoveUIPanel(bool isOpen)
        {
            float speed = 0.5f;
            RectTransform panelUI = DataModel.PanelUI;
            Vector3 curentPos = DataModel.PanelUI.position;
            Vector3 startPos = new Vector3(0, 0, 0) * DataModel.Canvas.scaleFactor;
           // Vector3 closePos = new Vector3(-panelUI.rect.width, 37, 0) * DataModel.Canvas.scaleFactor;
            Vector3 closePos = new Vector3(0, -panelUI.rect.height - 100, 0) * DataModel.Canvas.scaleFactor;
            Vector3 endPos = isOpen ? closePos : startPos;

            TweenController.MoveUIPanel(speed, panelUI, curentPos, endPos);

            DataModel.IsOpenUIPanel = !DataModel.IsOpenUIPanel;
        }

    }

}