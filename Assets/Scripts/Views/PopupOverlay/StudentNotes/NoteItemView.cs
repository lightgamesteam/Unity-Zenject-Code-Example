using TDL.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class NoteItemView : ViewBase
{
    public int noteID { get; set; }
    public Button noteButton { get; set; }
    public Button deleteNoteButton { get; set; }
    private Text _noteText { get; set; }
    private TextMeshProUGUI _timeStamp { get; set; }
    
    // Font size
    private float _defaultTimeStampFontSize;
    private int _defaultNoteFontSize;

    public override void InitUiComponents()
    {
        noteButton = GetComponent<Button>();
        deleteNoteButton = transform.Get<Button>("DeleteButton");
        _noteText = transform.Get<Text>("Text");
        _timeStamp = transform.Get<TextMeshProUGUI>("TimeStamp");

        SaveDefaultFontSizes();
    }

    private void SaveDefaultFontSizes()
    {
        _defaultTimeStampFontSize = _timeStamp.fontSize;
        _defaultNoteFontSize = _noteText.fontSize;        
    }

    private void Reset()
    {
        _noteText.text = string.Empty;
        _timeStamp.text = string.Empty;
        noteButton.onClick.RemoveAllListeners();
        deleteNoteButton.onClick.RemoveAllListeners();
    }
    
    public void SetTimeStampText(string text)
    {
        _timeStamp.text = text;
    }

    public void SetTimeStampText(string text, float fontScale)
    {
        _timeStamp.text = text;
        
        _timeStamp.fontSize = _defaultTimeStampFontSize;
        var currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
        if (currentFontSize != AccessibilityConstants.FontSizeMedium150)
        {
            _timeStamp.fontSize = Mathf.RoundToInt(_timeStamp.fontSize / fontScale);
        }
    }
    
    public string GetTimeStampText()
    {
        return _timeStamp.text;
    }
    
    public void SetNoteText(string text, float fontScale)
    {
        _noteText.text = text;
        
        _noteText.fontSize = _defaultNoteFontSize;
        var currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
        if (currentFontSize != AccessibilityConstants.FontSizeMedium150)
        {
            _noteText.fontSize = Mathf.RoundToInt(_noteText.fontSize / fontScale);
        }
    }
    
    public string GetNoteText()
    {
        return _noteText.text;
    }

    public class Pool : MonoMemoryPool<Transform, NoteItemView>
    {
        protected override void Reinitialize(Transform viewParent, NoteItemView view)
        {
            if (view.transform.parent == null)
            {
                view.transform.SetParent(viewParent, false);
            }

            view.Reset();
        }
    }
}