using UnityEngine;

namespace Module.PeriodicTable
{
    public class EventController : MonoBehaviour {

        public static void ClickOnTypeButton(GameObject gameObject)
        {
            PeriodicGroupController.ShowSelectGroup(gameObject);
        }

        public static void ClickOnElement(GameObject gameObject)
        {
            ApplicationView.instance.MoveElementToCenter(gameObject);

            if (SelectedModel.IsSelectedGroup())
            {
                PeriodicGroupController.ReturnSelectedGroup();
            }
        }

        public static void ClickOnBackground()
        {
            if (SelectedModel.IsSelectedElement())
            {
                ApplicationView.instance.ReturnSelectedElement();

                if (SelectedModel.IsSelectedGroup())
                {
                    PeriodicGroupController.ShowSelectGroup(SelectedModel.SelectType.gameObject);
                }
            }
            else if(SelectedModel.IsSelectedGroup())
            {
                PeriodicGroupController.ReturnSelectedGroup();
                SelectedModel.ClearSelectType();
            }
        }
    }
}
