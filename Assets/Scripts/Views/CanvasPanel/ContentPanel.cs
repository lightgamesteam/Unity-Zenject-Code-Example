using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ContentPanel : MonoBehaviour
{
    public CanvasPanel canvasPanel;

    public Transform container;
    public Selectable template;
    public TextMeshProUGUI textBoxTemplate;

    public List<Selectable> buttonsList = new List<Selectable>();
    
    private Dictionary<int, TextMeshProUGUI> _cachedButtonsWithId = new Dictionary<int, TextMeshProUGUI>();
    public Dictionary<int, TextMeshProUGUI> CachedButtonsWithId
    {
        get => _cachedButtonsWithId;
    }
   
    public Selectable CreateButton(string buttonName, int assetId = 0)
    {
        template.gameObject.SetActive(false);
        textBoxTemplate?.gameObject.SetActive(false);

        GameObject go = Instantiate(template.gameObject);
        go.SetActive(true);
        go.transform.SetParent(container, false);
        go.transform.localScale = Vector3.one;
        Selectable button = go.GetComponent<Selectable>();

        var buttonText = go.GetComponentInChildren<TextMeshProUGUI>();
        buttonText.text = buttonName;
        buttonsList.Add(button);
        
        if (assetId != 0 && !_cachedButtonsWithId.ContainsKey(assetId))
        {
            _cachedButtonsWithId.Add(assetId, buttonText);
        }
		
        return button;
    }

    public void SetTextBox(string text)
    {
        template.gameObject.SetActive(false);
        textBoxTemplate.gameObject.SetActive(true);

        textBoxTemplate?.SetText(text);
    }

    public void ClearCachedButtonsWithId()
    {
        _cachedButtonsWithId.Clear();
    }
}
