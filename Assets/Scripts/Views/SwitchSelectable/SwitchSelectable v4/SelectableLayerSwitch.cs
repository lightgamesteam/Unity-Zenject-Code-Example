using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableLayerSwitch : MonoBehaviour
{
	[SerializeField]
	public SelectableLayerAbstract mainLayer;
	
	private  List<SelectableLayerElements> allLayers = new List<SelectableLayerElements>();
	private Selectable lastSelectable;
	private EventSystem eventSystem;

	private void Awake()
	{
		eventSystem = EventSystem.current;
		if (mainLayer == null)
			mainLayer = gameObject.GetComponent<SelectableLayerAbstract>();
		
		mainLayer.GenerateLayer();
		allLayers.Add(mainLayer.GetLayer());
	}

	private void Update ()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				allLayers[allLayers.Count - 1].CurrentSelectableIndex = 
					SelectPreviousElement(allLayers[allLayers.Count - 1].CurrentSelectableIndex, 
						allLayers[allLayers.Count - 1].Selectables);
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				allLayers[allLayers.Count - 1].CurrentSelectableIndex = 
					SelectNextElement(allLayers[allLayers.Count - 1].CurrentSelectableIndex, 
						allLayers[allLayers.Count - 1].Selectables);
			}
		}
	}

	public void SelectLayer(SelectableLayerElements layer)
	{
		if (allLayers.Contains(layer))
		{
			allLayers.Remove(layer);
			SelectElement(allLayers[allLayers.Count - 1].CurrentSelectableIndex,
				allLayers[allLayers.Count - 1].Selectables);
		}
		else
		{
			allLayers.Add(layer);
			SelectElement(allLayers[allLayers.Count - 1].CurrentSelectableIndex,
				allLayers[allLayers.Count - 1].Selectables);
		}
	}

	public void AddLayer(SelectableLayerElements layer)
	{
		if (!allLayers.Contains(layer))
		{
			allLayers.Add(layer);
			SelectElement(allLayers[allLayers.Count - 1].CurrentSelectableIndex,
				allLayers[allLayers.Count - 1].Selectables);
		}
	}
	
	public void RemoveLayer(SelectableLayerElements layer)
	{
		if (allLayers.Contains(layer))
		{
			allLayers.Remove(layer);

			int index = allLayers[allLayers.Count - 1].CurrentSelectableIndex;
			index = CheckCurrentSelectedElement(index, allLayers[allLayers.Count - 1].Selectables);
			SelectElement(index, allLayers[allLayers.Count - 1].Selectables);
		}
	}

	private void SelectElement(int index, List<Selectable> fromLayer)
	{
		if (index >= 0)
		{
			lastSelectable = fromLayer[index];
			lastSelectable.Select();
		}
	}
	
	private int SelectNextElement(int index, List<Selectable> layer)
	{
		index = CheckCurrentSelectedElement(index, layer);
		index = IncrementLayerIndex(1, index, layer);

		SelectElement(index, layer);

		if (!layer[index].gameObject.activeSelf)
			SelectNextElement(index, layer);
		
		return index;
	}
	
	private int SelectPreviousElement(int index, List<Selectable> layer)
	{
		index = CheckCurrentSelectedElement(index, layer);
		index = IncrementLayerIndex(-1, index, layer);
		
		SelectElement(index, layer);
		
		if (!layer[index].gameObject.activeSelf)
			SelectPreviousElement(index, layer);
		
		return index;
	}

	private int IncrementLayerIndex(int by, int index, List<Selectable> layer)
	{
		index += by;
		
		if (index > layer.Count - 1 && by > 0)
			index = 0;
		
		if (index < 0 && by < 0)
			index = layer.Count - 1;

		return index;
	}

	private int CheckCurrentSelectedElement(int inputIndex, List<Selectable> layer)
	{
		//GameObject go = lastSelectable == null ? eventSystem.currentSelectedGameObject : lastSelectable.gameObject;
		
		GameObject go = eventSystem.currentSelectedGameObject;

		for (int i = 0; i < layer.Count; i++)
		{
			if (layer[i].gameObject.Equals(go))
				return i;
		}

		return inputIndex;
	}
}