using System.Collections.Generic;
using  System;
using UnityEngine.UI;

[Serializable]
public class SelectableLayerElements
{
	public int CurrentSelectableIndex;
	public List<Selectable> Selectables;

	public SelectableLayerElements(int currentSelectableIndex, List<Selectable> selectables)
	{
		CurrentSelectableIndex = currentSelectableIndex;
		Selectables = selectables;
	}
}