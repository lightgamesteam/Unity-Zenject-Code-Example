using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class SelectableLayerEvents : MonoBehaviour
{
	public SelectableLayerSwitch selectableLayerSwitch;
	public SelectableLayerAbstract selectableLayer;
	
	[Space(20f)] 
	public bool isAutoAddListenerToUI = true;
	
	[Space(20f)] 
	public LayerChangeEvent onSelectLayer = new LayerChangeEvent();
	
	[Serializable]
	public class LayerChangeEvent : UnityEvent<SelectableLayerElements>
	{
            
	}

	private void Awake()
	{
		if (selectableLayer == null)
			selectableLayer = gameObject.GetComponent<SelectableLayerAbstract>();
			
		if(!gameObject.HasComponent<Selectable>() && !isAutoAddListenerToUI)
			return;
		
		Selectable sl = gameObject.GetComponent<Selectable>();

		switch (sl)
		{
			case Button btn :
				btn.onClick.AddListener(SelectThisLayer);
				
				break;
			
			case Toggle tgl :
				tgl.onValueChanged.AddListener(b => { SelectThisLayer(); });
				
				break;

			case TMP_DropdownV2 drd:
				drd.onShowDropdown.AddListener(SelectThisLayerDropdown);
				
				break;
		}
	}

	private void SelectThisLayerDropdown(bool b)
	{
		if (b)
		{
			SelectThisLayer();
		}
		else
		{
			selectableLayerSwitch.RemoveLayer(selectableLayer.GetLayer());
		}
	}

	public void SelectThisLayer()
	{
		selectableLayer.GenerateLayer();
		onSelectLayer.Invoke(selectableLayer.GetLayer());
		selectableLayerSwitch.SelectLayer(selectableLayer.GetLayer());
	}
}
