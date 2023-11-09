using TMPro;
using UnityEngine;
using Zenject;

public class ModulesMenuItemView : ViewBase
{
	[SerializeField] 
	private TextMeshProUGUI _modulesName;
    
	[SerializeField] 
	private GameObject _modulesContent;

	public void SetModulesName(string modules)
	{
		_modulesName.text = modules;
	}

	public Transform GetModulesContent()
	{
		return _modulesContent.transform;
	}

	public class Factory : PlaceholderFactory<ModulesMenuItemView>
	{
	}
}