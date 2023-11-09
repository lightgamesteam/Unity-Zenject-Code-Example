using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class SelectableLayerDropdown : SelectableLayerAbstract
{
    public override SelectableLayerElements GetLayer()
    {
        return selectablesLayer;
    }

    public override void GenerateLayer()
    {
        selectablesLayer.Selectables.Clear();
        selectablesLayer.Selectables.AddRange(new List<Selectable>(gameObject.GetComponentsInChildren<Selectable>()));
        
        if( selectablesLayer.Selectables.Contains(GetComponent<Selectable>()))
            selectablesLayer.Selectables.Remove(GetComponent<Selectable>());
    }
}