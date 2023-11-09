using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class DropdownToggleExtension
{
    public static Toggle AddLabelToDropdownToggleLost(this DropdownToggle ddt, string modelName, string localizedText, GameObject label, bool isAddToShowAll = true)
    {
        GameObject go = MonoBehaviour.Instantiate(ddt.Template.gameObject, ddt.ToggleGroupContainer.transform, false);
        go.transform.localScale = Vector3.one;
        Toggle tgl = go.GetComponent<Toggle>();
        tgl.isOn = false;
        tgl.onValueChanged.AddListener(label.SetActive);
        label.SetActive(false);
		
        go.GetComponentInChildren<TextMeshProUGUI>().text = localizedText;
		
        if (isAddToShowAll)
        {
            ddt.Template.GetComponent<Toggle>().onValueChanged.AddListener((value) => { go.GetComponent<Toggle>().isOn = value; });
            ddt.Template.GetComponent<Toggle>().isOn = false;
        }

        return tgl;
    }

    public static void ClearLabelsInDropdownToggleLost(this DropdownToggle ddt)
    {
        ddt.Template.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();

        Transform child = ddt.ToggleGroupContainer.transform;
        
        for (int i = 1; i < child.childCount; i++)
        {
            MonoBehaviour.Destroy(child.GetChild(i).gameObject);
        }
    }
}
