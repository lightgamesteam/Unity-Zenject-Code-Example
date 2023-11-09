using UnityEngine;

namespace Module.PeriodicTable
{
    public class PeriodicGroupView : MonoBehaviour
    {
        void OnMouseDown()
        {
            EventController.ClickOnTypeButton(gameObject);
        }
    }
}