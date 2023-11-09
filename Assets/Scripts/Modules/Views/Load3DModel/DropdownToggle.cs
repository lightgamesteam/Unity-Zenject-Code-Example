using System.Collections.Generic;
using System.Linq;
using Module.Core.UIComponent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownToggle : MonoBehaviour
{
    public Toggle DropdownListToggle;
    public GameObject SearchModelPanel;
    public TextMeshProUGUI DropdownLabel;
    public ToggleGroup ToggleGroupContainer;
    public GameObject Template;
    public TextMeshProUGUI TemplateLabel;
    
    [Header("Search panel")]
    public Button ToggleSearchButton;
    public Button SearchCloseButton;
    public InputField SearchInputField;
    
    public void ScaleFontSize(float scale)
    {
        if(DropdownLabel)
            DropdownLabel.fontSize /= scale;

        if(TemplateLabel)
            TemplateLabel.fontSize /= scale;

        if (SearchInputField)
        {
            SearchInputField.gameObject.GetAllComponentsInChildren<Text>().ForEach(t =>
            {
                t.fontSize = (int) (t.fontSize / scale);
            });
        }
    }
    
    public void SetDropdownToggleItem(Toggle dropdownListToggle, ToggleGroup toggleGroupContainer, GameObject template)
    {
        DropdownListToggle = dropdownListToggle;
        ToggleGroupContainer = toggleGroupContainer;
        Template = template;
    }

    public void SortToggleItemByText(bool includeTemplate = false, bool isMobileLabel = false)
    {
        List<TextMeshProUGUI> textMP = new List<TextMeshProUGUI>();
        textMP.AddRange(ToggleGroupContainer.transform.GetComponentsInChildren<TextMeshProUGUI>());
        
        List<string> itemName = new List<string>();
        
        if(!isMobileLabel)
        {
            itemName.AddRange(textMP.ConvertAll(i => i.text));
        }
        else
        {
            itemName.AddRange(textMP.ConvertAll(i =>
            {
                string txt = i.text.Split('.')[1];
                i.text = txt;  
                return txt;
            }));
        }

        int shiftindex = includeTemplate ? 0 : 1;
        
        if(!includeTemplate)
            itemName.Remove(Template.GetComponentInChildren<TextMeshProUGUI>().text);
        
        itemName.Sort();
        
        itemName.ForEach(itm =>
        {
            TextMeshProUGUI tm = textMP.Find(tmp => tmp.text == itm);
            int index = itemName.IndexOf(itm) + shiftindex;
            
            if(isMobileLabel)
                tm.text = $"{index}. {tm.text}";
            
            Transform tr = tm.transform.parent;
            
            tr.SetSiblingIndex(index);
        });
    }

    public void SetAllowSwitchOff(bool status)
    {
        ToggleGroupContainer.allowSwitchOff = status;
    }
    
    internal void SetAllToggleIsOn(bool isOn)
    {
        List<Toggle> tgls = new List<Toggle>(ToggleGroupContainer.transform.GetComponentsInChildren<Toggle>(true));
        tgls.ForEach(t => t.SetValue(isOn, true));
    }

    public T CreateTemplate<T>(string title)
    {
        GameObject go = Instantiate(Template.gameObject, ToggleGroupContainer.transform, false);
        go.SetActive(true);
        go.name = title;
        go.transform.localScale = Vector3.one;
        go.transform.SetPositionZ(Template.transform.position.z);
        go.GetComponentInChildren<TextMeshProUGUI>().text = title;
        
        T tmp = go.GetComponent<T>();
		
        return tmp;
    }
    
    public GameObject CreateTemplate(string title)
    {
        GameObject go = Instantiate(Template.gameObject, ToggleGroupContainer.transform, false);
        go.SetActive(true);
        go.name = title;
        go.transform.localScale = Vector3.one;
        go.transform.SetPositionZ(Template.transform.position.z);
        go.GetComponentInChildren<TextMeshProUGUI>().text = title;
		
        return go;
    }
    
    public GameObject CreateTemplate(string title, Sprite spriteType)
    {
        GameObject go = Instantiate(Template.gameObject, ToggleGroupContainer.transform, false);
        go.transform.GetComponentByName<Image>("Type").sprite = spriteType;
        go.SetActive(true);
        go.name = title;
        go.transform.localScale = Vector3.one;
        go.transform.SetPositionZ(Template.transform.position.z);
        go.GetComponentInChildren<TextMeshProUGUI>().text = title;
		
        return go;
    }
    
    public Button CreateAssociatedContentButton(string title)
    {
        Template.gameObject.SetActive(false);
        GameObject go = Instantiate(Template.gameObject, ToggleGroupContainer.transform, false);
        go.SetActive(true);
        go.name = title;
        go.transform.localScale = Vector3.one;
        go.transform.SetPositionZ(Template.transform.position.z);
        go.GetComponentInChildren<TextMeshProUGUI>().text = title;

        Button tmp = go.transform.Get<Button>("Item_Button");
        
        return tmp;
    }
    
    public Toggle CreateLabelToggle(string title, GameObject label, bool isAddToShowAll = true)
    {
        GameObject go = Instantiate(Template.gameObject, ToggleGroupContainer.transform, false);
        go.SetActive(true);
        go.name = title;
        go.transform.localScale = Vector3.one;
        go.transform.SetPositionZ(Template.transform.position.z);
        Toggle tgl = go.GetComponent<Toggle>();
        tgl.isOn = false;
        
        if (label)
        {
            tgl.onValueChanged.AddListener(label.SetActive);
            label.SetActive(false);
        }
		
        go.GetComponentInChildren<TextMeshProUGUI>().text = title;
		
        if (isAddToShowAll)
        {
            Template.GetComponent<Toggle>().onValueChanged.AddListener(value => { go.GetComponent<Toggle>().isOn = value; });
            Template.GetComponent<Toggle>().isOn = false;
        }

        return tgl;
    }

    public void ClearDropdownToggle()
    {
        Transform child = ToggleGroupContainer.transform;
        
        for (int i = 1; i < child.childCount; i++)
        {
             Destroy(child.GetChild(i).gameObject);
        }

        ClearTemplateListeners();
    }

    public void ClearTemplateListeners()
    {
        Selectable[] sel = Template.gameObject.GetComponentsInChildren<Selectable>(true);

        foreach (Selectable selectable in sel)
        {
            switch (selectable)
            {
                case Button btn:
                    btn.onClick.RemoveAllListeners();
                    break;
                
                case Toggle tgl:
                    tgl.onValueChanged.RemoveAllListeners();
                    break;
            }
        }
    }
    
}
