using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ClickOutsideHideAll : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private bool onlyOnThisLayer = false;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (onlyOnThisLayer)
        {
            gameObject.GetAllInSceneOnLayer<ClickOutsideTrigger>().ForEach(t =>
            {
                t.InvokeTrigger();
            });
        }
        else
        {
            gameObject.GetAllInScene<ClickOutsideTrigger>().ForEach(t =>
            {
                t.InvokeTrigger();
            });
        }
    }
}