using UnityEngine.EventSystems;

public interface ISelectableMenuItem : IPointerEnterHandler, IPointerExitHandler
{
    void Select();
    void Deselect();
    bool IsSelected();
}