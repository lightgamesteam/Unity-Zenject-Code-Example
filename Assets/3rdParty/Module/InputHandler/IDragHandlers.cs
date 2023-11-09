using UnityEngine.EventSystems;

namespace Module.InputHandler {
    public interface IDragHandlers : IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {}
}
