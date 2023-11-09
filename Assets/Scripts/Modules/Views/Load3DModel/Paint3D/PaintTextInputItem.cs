using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XDPaint;

public class PaintTextInputItem : MonoBehaviour
{
    public bool isOnDrag;


    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private GameObject topPanel;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI placeholderText;

    public TMP_InputField inputField { get => _inputField; private set => _inputField = value; }

    private Camera myCamera;

    public EventTrigger moveEventTrigger { get; private set; }
    public Button deleteButton { get; private set; }
    public static Action<Color> SetCurrTextColor = delegate {};
    [HideInInspector] public PaintManager3DL myPaintManager;
    
    private bool isSelected;

    public void Awake()
    {
        GetReference();
    }

    private void GetReference()
    {
        if (inputField == null)
        {
            inputField = gameObject.GetComponentInChildren<TMP_InputField>();
        }
        if (topPanel == null)
        {
            topPanel = transform.Find("X_Top_Panel").gameObject;
        }
        if (text == null)
        {
            text = inputField.transform.Get<TextMeshProUGUI>("Root/Text");
        }
        if (placeholderText == null)
        {
            placeholderText = inputField.placeholder.GetComponent<TextMeshProUGUI>();
        }
        if (moveEventTrigger == null)
        {
            moveEventTrigger = topPanel.transform.Get<EventTrigger>("Move");
        }
        if (deleteButton == null)
        {
            deleteButton = topPanel.transform.Get<Button>("Destroy");
        }
        if (myCamera == null)
        {
            myCamera = inputField.GetComponent<RectTransform>().GetMyCanvas().worldCamera;
        }

        deleteButton.onClick.RemoveListener(gameObject.SelfDestroy);
        deleteButton.onClick.AddListener(gameObject.SelfDestroy);

        SetCurrTextColor -= TextColorHandler;
        SetCurrTextColor += TextColorHandler;
    }

    private void TextColorHandler(Color color)
    {
        if (isSelected)
        {
            SetTextColor(color);
        }
    }

    public bool IsVisible()
    {
        GetReference();
        return Vector3.Dot(transform.forward, myCamera.transform.forward) > 0.85f;
    }

    public void ShowTopPanel(bool isActive)
    {
        isSelected = isActive;
        topPanel.SetActive(isActive);
    }
    
    public void SetTextColor(Color textColor)
    {
        inputField.caretColor = textColor;
        inputField.textComponent.color = textColor;
        placeholderText.color = textColor;
    }

    public void SetInteractable(bool isActive)
    {
        if (inputField == null)
            GetReference();
        
        inputField.interactable = isActive;
        
        text.raycastTarget = isActive;
        text.enableCulling = true;
        text.ForceMeshUpdate();
        
        placeholderText.raycastTarget = isActive;
        placeholderText.enableCulling = true;
        placeholderText.ForceMeshUpdate();
        
        gameObject.GetComponentsInChildren<Image>(true).ToList().ForEach(img =>
        {
            img.raycastTarget = isActive;
        });
    }
}
