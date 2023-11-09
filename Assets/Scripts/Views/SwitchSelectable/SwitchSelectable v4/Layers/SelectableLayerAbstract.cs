using UnityEngine;
using UnityEngine.UI;
using  System.Collections.Generic;
public abstract class SelectableLayerAbstract : MonoBehaviour
{
    public bool resetStarIndexOnEnable = false;
    public int resetToStartIndex;
    public SelectableLayerElements selectablesLayer;

    private void OnEnable()
    {
        if (resetStarIndexOnEnable)
        {
            selectablesLayer.CurrentSelectableIndex = resetToStartIndex;
        }
    }

    public virtual SelectableLayerElements GetLayer()
    {
        return selectablesLayer;
    }
  
    public virtual void GenerateLayer()
    {
        if (selectablesLayer.Selectables.Count == 0)
        {
            selectablesLayer = new SelectableLayerElements(selectablesLayer.CurrentSelectableIndex,
                new List<Selectable>(gameObject.GetComponentsInChildren<Selectable>()));
        }
    }
}
