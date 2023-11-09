using TDL.Signals;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TDL.Views
{
    public class RightPanelItemView : ViewBase
    {
        private const string Left_Menu_Toggle = "LeftMenuToggle";
        private const string Right_Menu_Toggle = "RightMenuToggle";
    
        private RectTransform _rectPanel;
        public bool IsInside { get; private set; }

        [SerializeField] private LeftPanelItemView _leftPanel;

        private void Start()
        {
            _rectPanel = GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var clickedObject = EventSystem.current.currentSelectedGameObject;

                if (clickedObject != null)
                {
                    if (clickedObject.name.Equals(Left_Menu_Toggle))
                    {
                        Signal.Fire(new ShowRightMenuCommandSignal(false));
                        return;
                    }

                    if (clickedObject.name.Equals(Right_Menu_Toggle))
                        return;
                }
            
                if (!IsInside && (!_leftPanel.isActiveAndEnabled || !_leftPanel.IsInside))
                {
                    HideSideMenus();
                }
            }

            var mousePos = Input.mousePosition;
            IsInside = RectTransformUtility.RectangleContainsScreenPoint(_rectPanel, mousePos, Camera.main);
        }

        private void HideSideMenus()
        {
            Signal.Fire(new ShowRightMenuCommandSignal(false));
            Signal.Fire(new ShowLeftMenuCommandSignal(false));
        }

        public void OnKeyboardNavigationOut()
        {
            Signal.Fire(new ShowRightMenuCommandSignal(false));
        }
    }
}