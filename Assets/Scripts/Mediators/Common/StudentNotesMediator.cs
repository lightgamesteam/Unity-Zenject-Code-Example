using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using UnityEngine;
using Zenject;

public class StudentNotesMediator : IInitializable, IDisposable 
{
    [Inject] private readonly SignalBus _signal;
    [Inject] private StudentNotesView.Factory _viewFactory;
    [Inject] private NoteItemView.Pool _noteItemPool;
    [Inject] private LocalizationModel _localization;
    [Inject] private ContentModel _contentModel;
    [Inject] private AsyncProcessorService _asyncProcessor;
    [Inject] private readonly AccessibilityModel _accessibilityModel;

    private StudentNotesView _view;
    private string _cultureCode;
    private int _assetID = -1;
    private int _gradeId,  _subjectId, _topicId, _subtopicId = -1;

    private List<NoteItemView> _allNotesInView = new List<NoteItemView>();
    
    public void Initialize()
    {
        SubscribeSignals();
        CreatePanelView();
        AddUIListeners();
    }

    void SubscribeSignals()
    {
        _signal.Subscribe<ShowStudentNotesPanelViewSignal>(ShowPanel);
        _signal.Subscribe<AddNoteSpawnSignal>(s => AddNoteItem(s.AssetId, s.NoteId));
        _signal.Subscribe<GetAllNotesSpawnSignal>(s => SpawnAllNotes(s.NoteResponse));
    }
    
    void AddUIListeners()
    {
        _view.closeButton.onClick.AddListener(HidePanelView);
        _view.addNoteButton.onClick.AddListener(AddNoteSignal);
    }

    void CreatePanelView()
    {
        _view = _viewFactory.Create();
        _view.transform.SetSiblingIndex(0);
        _view.gameObject.SetActive(false);
    }

    void ShowPanel(ShowStudentNotesPanelViewSignal signal)
    {
        _allNotesInView.ForEach(n =>
        {
            if (n.gameObject.activeSelf)
                _noteItemPool.Despawn(n);
        });
        _allNotesInView.Clear();
        
        _assetID = signal.AssetID;

        var assetCat = _contentModel.GetAssetById(_assetID).Categories;
        _gradeId = assetCat.First()[ContentModel.CategoryNames.GradeId];
        _subjectId = assetCat.First()[ContentModel.CategoryNames.SubjectId];
        _topicId = assetCat.First()[ContentModel.CategoryNames.TopicId];
        _subtopicId = assetCat.First()[ContentModel.CategoryNames.SubtopicId];   
        
        _signal.Fire(new GetAllNotesCommandSignal(_assetID));
        ShowPanelView();
        UpdateTranslation(signal.CultureCode);
    }

    void UpdateTranslation(string cultureCode)
    {
        _cultureCode = cultureCode ?? _localization.CurrentLanguageCultureCode;
        
        _view.panelTitleText.text = _localization.GetSystemTranslations(_cultureCode, LocalizationConstants.NotesKey);
        
        UpdateTooltips(_view.gameObject);
        UpdateFontSize();
    }
    
    private void UpdateTooltips(GameObject go)
    {
        go.gameObject.GetAllComponentsInChildren<TooltipEvents>().ForEach(t =>
        {
            var keyTranslation = t.GetKey();

            if (!string.IsNullOrEmpty(keyTranslation))
            {
                t.SetHint(_localization.GetSystemTranslations(_cultureCode, keyTranslation));
            }
        });
    }

    private void UpdateFontSize()
    {
        _view.panelTitleText.fontSize = _view.DefaultPanelTitleFontSize;
        _view.editorPanelTitleText.fontSize = _view.DefaultEditorPanelTitleFontSize;
        _view.noteInput.textComponent.fontSize = _view.DefaultNoteInputFontSize;
        _view.noteInputPlaceholder.fontSize = _view.DefaultNoteInputPlaceholderFontSize;

        var currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
        if (currentFontSize != AccessibilityConstants.FontSizeMedium150)
        {
            _view.panelTitleText.fontSize = Mathf.RoundToInt(_view.panelTitleText.fontSize / _accessibilityModel.ModulesFontSizeScaler);
            _view.editorPanelTitleText.fontSize = Mathf.RoundToInt(_view.editorPanelTitleText.fontSize / _accessibilityModel.ModulesFontSizeScaler);
            _view.noteInput.textComponent.fontSize = Mathf.RoundToInt(_view.noteInput.textComponent.fontSize / _accessibilityModel.ModulesFontSizeScaler);
            _view.noteInputPlaceholder.fontSize = Mathf.RoundToInt(_view.noteInputPlaceholder.fontSize / _accessibilityModel.ModulesFontSizeScaler);
        }
    }
    
    void ShowPanelView()
    {
        _view.ShowView();
    }
    
    void HidePanelView()
    {
        _view.HideView();
    }

    void AddNoteSignal()
    {
        _signal.Fire(new AddNoteCommandSignal("", _assetID, _gradeId, _subjectId, _topicId, _subtopicId));
    }

    void AddNoteItem(int assetId, int noteId)
    {
        if(assetId != _assetID)
            return;
        
        NoteItemView n = SpawnNote(noteId);
        n.SetTimeStampText(DateTime.Now.ToString(CultureInfo.CurrentCulture), _accessibilityModel.ModulesFontSizeScaler);
        
        if (n)
        {
            OpenEditNote(n);
            StartEditNote(n);
        }
    }

    private void SpawnAllNotes(NoteResponse nr)
    {
        if(nr.assetNote.Length == 0)
            return;
        
        if(nr.assetNote.First().assetId == _assetID)
        {
            foreach (AssetNoteModel noteModel in nr.assetNote)
            {
                NoteItemView ni = SpawnNote(noteModel.noteId);
                ni.SetNoteText(noteModel.notes, _accessibilityModel.ModulesFontSizeScaler);

                DateTime dt = Convert.ToDateTime(noteModel.createdOn);
                dt = TimeZoneInfo.ConvertTimeFromUtc(dt, TimeZoneInfo.Local);
                
                ni.SetTimeStampText(dt.ToString(CultureInfo.CurrentCulture), _accessibilityModel.ModulesFontSizeScaler);
            }
        }
    }
    private NoteItemView SpawnNote(int noteId)
    {
        NoteItemView ni = _noteItemPool.Spawn(_view.noteContainer);
        ni.gameObject.transform.SetAsFirstSibling();
        ni.SetTimeStampText("");
        ni.noteID = noteId;
        UpdateTooltips(ni.gameObject);
        _allNotesInView.Add(ni);

        ni.deleteNoteButton.onClick.AddListener(() => DeleteNote(ni));
        ni.noteButton.onClick.AddListener(() => OpenEditNote(ni));

        return ni;
    }
    
    void OpenEditNote(NoteItemView ni)
    {
        SetEditorIsOpen(true);
        SetEditorState(false);
        _view.noteInput.text = ni.GetNoteText();
        _view.editorPanelTitleText.text = ni.GetTimeStampText();
        
        InitEditorListeners(ni);
    }

    void CloseEditNote(NoteItemView ni)
    {
        SetEditorIsOpen(false);

        if (string.IsNullOrEmpty(ni.GetNoteText()))
            DeleteNote(ni);
    }
    
    void StartEditNote(NoteItemView ni)
    {
        SetEditorState(true);
    }
    
    void ApplyEditNote(NoteItemView ni)
    {
        SetEditorState(false);
        _signal.Fire(new UpdateNoteCommandSignal(ni.noteID, _assetID, _view.noteInput.text));
        ni.SetNoteText(_view.noteInput.text, _accessibilityModel.ModulesFontSizeScaler);
    }
    
    void DeleteNote(NoteItemView ni)
    {
        _signal.Fire(new DeleteNoteCommandSignal(ni.noteID));
        _allNotesInView.Remove(ni);
        _noteItemPool.Despawn(ni);
    }
    
    private void InitEditorListeners(NoteItemView ni)
    {
        _view.applyEditButton.onClick.RemoveAllListeners();
        _view.startEditButton.onClick.RemoveAllListeners();
        _view.cancelEditButton.onClick.RemoveAllListeners();

        _view.applyEditButton.onClick.AddListener(() => ApplyEditNote(ni));
        _view.startEditButton.onClick.AddListener(() => StartEditNote(ni));
        _view.cancelEditButton.onClick.AddListener(() => CloseEditNote(ni));
    }
    
    void SetEditorIsOpen(bool isOpen)
    {
        _view.mainPanel.gameObject.SetActive(!isOpen);
        _view.editorPanel.gameObject.SetActive(isOpen);
    }
    
    void SetEditorState(bool isEditState)
    {
        _view.noteInput.interactable = isEditState;
        _view.applyEditButton.gameObject.SetActive(isEditState);
        _view.startEditButton.gameObject.SetActive(!isEditState);
    }
    
    public void Dispose()
    {
    }
}
