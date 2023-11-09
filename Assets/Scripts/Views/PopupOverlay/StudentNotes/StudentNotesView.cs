using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StudentNotesView : ViewBase, IAnimatedView
{
    // Main Panel 
    public TextMeshProUGUI panelTitleText { get; set; }
    public Button closeButton { get; set; }
    public Button addNoteButton { get; set; }
    public Transform noteContainer { get; set; }
    
    // Editor Panel 
    public GameObject editorPanel { get; set; }
    public TextMeshProUGUI editorPanelTitleText { get; set; }
    public TMP_InputField noteInput { get; set; }
    public TextMeshProUGUI noteInputPlaceholder { get; set; }
    public Button cancelEditButton { get; set; }
    public Button startEditButton { get; set; }
    public Button applyEditButton { get; set; }

    // Animation 
    public CanvasGroup canvasGroup { get; set; }
    public Transform mainPanel { get; set; }
    
    // Font size
    public float DefaultPanelTitleFontSize { get; set; }
    public float DefaultEditorPanelTitleFontSize { get; set; }
    public float DefaultNoteInputFontSize { get; set; }
    public float DefaultNoteInputPlaceholderFontSize { get; set; }

    public override void InitUiComponents()
    {
        // main panel
        panelTitleText = transform.Get<TextMeshProUGUI>("Panel/Top/Title");
        closeButton = transform.Get<Button>("Panel/Top/Button_Close");
        addNoteButton = transform.Get<Button>("Panel/Top/Button_Add");
        noteContainer = transform.Find("Panel/NotesPanel/Scroll View/Viewport/Content");
        
        // editor panel
        editorPanel = transform.Find("X_EditorPanel").gameObject; 
        editorPanelTitleText = transform.Get<TextMeshProUGUI>("X_EditorPanel/Top/Title");
        noteInput = transform.Get<TMP_InputField>("X_EditorPanel/NotesPanel/Note");
        noteInputPlaceholder = noteInput.transform.Get<TextMeshProUGUI>("Text Area/Placeholder");
        cancelEditButton = transform.Get<Button>("X_EditorPanel/Top/Button_Back");
        startEditButton = transform.Get<Button>("X_EditorPanel/Top/Button_Edit");
        applyEditButton = transform.Get<Button>("X_EditorPanel/Top/X_Button_Apply");
        
        // animation
        canvasGroup = GetComponent<CanvasGroup>();
        mainPanel = transform.Find("Panel");
        
        SaveDefaultFontSizes();
    }

    private void SaveDefaultFontSizes()
    {
        DefaultPanelTitleFontSize = panelTitleText.fontSize;
        DefaultEditorPanelTitleFontSize = editorPanelTitleText.fontSize;
        DefaultNoteInputFontSize = noteInput.textComponent.fontSize;
        DefaultNoteInputPlaceholderFontSize = noteInputPlaceholder.fontSize;
    }

    public void ShowView()
    {
        EnableView(true);
        Signal.Fire(new FocusKeyboardNavigationSignal(canvasGroup, true));
        
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.3f).SetEase(Ease.OutSine);

        mainPanel.localScale = Vector3.zero;
        mainPanel.DOScale(1, 0.3f).SetEase(Ease.OutSine);
    }

    public void HideView()
    {
        Signal.Fire(new FocusKeyboardNavigationSignal(canvasGroup, false));

        canvasGroup.DOFade(0, 0.3f).SetEase(Ease.OutSine);
        mainPanel.DOScale(0, 0.3f).SetEase(Ease.OutSine).onComplete += () => { EnableView(false); };
    }
    
    private void EnableView(bool isEnabled)
    {
        gameObject.SetActive(isEnabled);
    }

    public class Factory : PlaceholderFactory<StudentNotesView>
    {
    }
}
