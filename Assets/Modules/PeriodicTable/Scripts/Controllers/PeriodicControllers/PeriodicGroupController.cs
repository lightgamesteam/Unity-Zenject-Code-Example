using TDL.Commands;
using UnityEngine;

namespace Module.PeriodicTable
{
    public class PeriodicGroupController : MonoBehaviour
    {
        public static void ShowSelectGroup(GameObject selectedGroup)
        {
            SelectedModel.openGroup = true;
            SelectedModel.SelectType = PeriodicTableController.GetType(selectedGroup);
            if (SelectedModel.SelectType.name != null)
            {
                ApplicationView.instance.Signal.Fire(new AccessibilityTTSPlayCommandSignal(SelectedModel.SelectType.name.Replace("_", " ")));
            }
            MoveGroupToForward();
        }

        public static void SwithSelectGroup(GameObject selectedGroup)
        {
            if (SelectedModel.openGroup)
            {
                ReturnSelectedGroup();
            }
            else
            {
                SelectedModel.openGroup = true;
                SelectedModel.SelectType = PeriodicTableController.GetType(selectedGroup);
                MoveGroupToForward();
            }
        }

        public static void MoveGroupToForward()
        {
            var elements = PeriodicTableController.GetElements(SelectedModel.SelectType.button);

            TweenController.TweenColorImage(ApplicationView.instance.ImageBackground, DataModel.VisibleColor,
                DataModel.SpeedTweenColorBackground, true);
            ApplicationView.SetActiveCollider(SelectedModel.SelectType.button, false);

            var currentPos = SelectedModel.SelectType.button.transform.position;
            var startPos = SelectedModel.SelectType.positionButton;

            if (startPos == currentPos)
            {
                AnimationController.MoveElementToForward(SelectedModel.SelectType.button);
            }

            foreach (PeriodicModel.PeriodicElement periodicElement in elements)
            {
                if (!periodicElement.gameObject.Equals(SelectedModel.SelectElement.gameObject))
                {
                    ApplicationView.SetActiveCollider(periodicElement.gameObject, false);
                    AnimationController.MoveElementToForward(periodicElement.gameObject);
                }
            }
        }

        public static void ReturnSelectedGroup()
        {
            var elements = PeriodicTableController.GetElements(SelectedModel.SelectType.button);
            var selectType = PeriodicTableController.GetType(SelectedModel.SelectType.button);

            if (SelectedModel.IsSelectedElement())
            {
                TweenController.TweenColorImage(ApplicationView.instance.ImageBackground, DataModel.VisibleColor,
                    DataModel.SpeedTweenColorBackground, true);
            }
            else
            {
                TweenController.TweenColorImage(ApplicationView.instance.ImageBackground, DataModel.InvisibleColor,
                    DataModel.SpeedTweenColorBackground, false);
                AnimationController.ReturnElement(SelectedModel.SelectType.button, selectType.positionButton);
            }


            foreach (PeriodicModel.PeriodicElement periodicElement in elements)
            {
                if (!SelectedModel.SelectElement.Equals(periodicElement))
                {
                    AnimationController.ReturnElement(periodicElement.gameObject, periodicElement.position);
                }
            }
        }
    }
}