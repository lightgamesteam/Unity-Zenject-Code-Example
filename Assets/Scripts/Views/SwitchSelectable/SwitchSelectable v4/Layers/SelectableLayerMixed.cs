using System.Collections.Generic;
using UnityEngine.UI;

public class SelectableLayerMixed : SelectableLayerAbstract
{
	public int startIndex;
	public SelectableLayerSwitch selectableLayerSwitch;
	private bool isGenerated = false;
	private List<Selectable> selectableInChild;
	public override SelectableLayerElements GetLayer()
	{
		selectablesLayer.CurrentSelectableIndex = startIndex;
		return selectablesLayer;
	}

	public override void GenerateLayer()
	{
		if (!isGenerated)
		{
			selectableInChild = new List<Selectable>(gameObject.GetComponentsInChildren<Selectable>());
			selectablesLayer.Selectables.AddRange(selectableInChild);

			foreach (Selectable s in selectableInChild)
			{
				SelectableLayerEvents sle = s.gameObject.AddComponent<SelectableLayerEvents>();
				sle.selectableLayer = this;
				sle.onSelectLayer.AddListener(selectableLayerSwitch.RemoveLayer);
			}
			
			isGenerated = true;
		}
	}
}