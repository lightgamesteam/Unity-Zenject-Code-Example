using System;
using System.Collections.Generic;
using System.Linq;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using TMPro;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class ContentHomeViewMediator : ContentViewMediatorBase, IInitializable, IDisposable, IMediator
    {
        [Inject] protected ContentHomeView.Factory _contentHomeViewFactory;
        [Inject] private ContentViewMediator _contentViewMediator;
        [Inject] private UserLoginModel _loginModel;
        
        private ContentHomeView _view => _contentViewModel.contentHomeView;
        private ContentHomeActivitiesView _activitiesPanel => _view.contentHomeActivitiesPanel;
        private ContentHomeRelatedView _relatedPanel => _view.contentHomeRelatedPanel;

        public void Initialize()
        {
            InitializeModuleAction += InitializeModule;
            InitializeARAction += InitializeAR;
            _contentViewModel.contentHomeView = _contentHomeViewFactory.Create();
            
            _view.InitUiComponents();
            
            SetupHomeView();
        }

        private void InitializeModule(int id)
        {
            SetupActivitiesView();
            SetupRelatedView();
            SetupLessonModeView();
            UpdateLocalization();
        }

        private void InitializeAR(bool isInit)
        {
            if (isInit)
            {
                //_contentViewModel.contentViewPC.homeButton.gameObject.SetActive(false);
                //ExploreMode();
            }
        }
        
        private void UpdateLocalization()
        {
            // Home Panel
            _view.activitiesButton.GetComponentInChildren<TextMeshProUGUI>().text = GetCurrentSystemTranslations(LocalizationConstants.Content3DModelTestKnowledgeKey);
            _view.relatedButton.GetComponentInChildren<TextMeshProUGUI>().text = GetCurrentSystemTranslations(LocalizationConstants.Content3DModelRelatedContentKey);
            _view.explainButton.GetComponentInChildren<TextMeshProUGUI>().text = GetCurrentSystemTranslations(LocalizationConstants.Content3DModelVideoModeKey);
            _view.exploreButton.GetComponentInChildren<TextMeshProUGUI>().text = GetCurrentSystemTranslations(LocalizationConstants.Content3DModelExploreKey);
            _view.lessonButton.GetComponentInChildren<TextMeshProUGUI>().text = GetCurrentSystemTranslations(LocalizationConstants.Content3DModelLessonModeKey);
            
            // Activities Panel
            _activitiesPanel.classificationPanelName.text = GetCurrentSystemTranslations(LocalizationConstants.ActivityClassificationsKey);
            _activitiesPanel.puzzlePanelName.text = GetCurrentSystemTranslations(LocalizationConstants.ActivityPuzzlesKey);
            _activitiesPanel.quizPanelName.text = GetCurrentSystemTranslations(LocalizationConstants.ActivityQuizzesKey);
            
            // Related Panel
            _relatedPanel.modelRelatedPanelName.text = GetCurrentSystemTranslations(LocalizationConstants.RelatedContentKey);
            _relatedPanel.labelRelatedPanelName.text = $"{GetCurrentSystemTranslations(LocalizationConstants.LabelsKey)} {GetCurrentSystemTranslations(LocalizationConstants.RelatedContentKey)}";
            
            // Lesson Panel
            
            ContentLocalizedLabel.ChangeLanguageAction?.Invoke(_contentViewModel.CurrentContentViewCultureCode);
        }

        #region MyRegion

        private void SetupLessonModeView()
        {
            
        }

        #endregion
        
        #region Activities
        
        private void SetupActivitiesView()
        {
            SetupQuizPanel();
            SetupPuzzlePanel();
            SetupClassificationPanel();
            
            if(!_activitiesPanel.classificationPanel.activeSelf && !_activitiesPanel.quizPanel.activeSelf && !_activitiesPanel.puzzlePanel.activeSelf)
                _view.activitiesButton.gameObject.SetActive(false);
            else
                _view.activitiesButton.gameObject.SetActive(true);

        }

        private void SetupClassificationPanel()
        {
            List<ClientActivityModel> classification = new List<ClientActivityModel>();
            
            _contentModel.Classifications.ForEach(mq =>
            {
                if (mq.ActivityItem.assetContent.FindAll(ac => ac.assetId == _contentViewModel.mainAssetID).Count > 0)
                {
                    classification.Add(mq);
                }
            });
            
            if (classification.Count > 0)
            {
                _activitiesPanel.classificationPanel.SetActive(true);
                CreateClassificationList();
            }
            else
            {
                _activitiesPanel.classificationPanel.SetActive(false);
            }

            void CreateClassificationList()
            {
                _activitiesPanel.classificationTemplate.gameObject.DestroyNeighbors();

                classification.ForEach(c =>
                {
                    TextTooltipEvents tt = _activitiesPanel.classificationTemplate.gameObject.Duplicate<TextTooltipEvents>();
                    tt.gameObject.SetActive(true);
                    tt.textMeshPro.text = GetCurrentTranslationsForItem(c.ActivityItem.activityLocal);
                    tt.gameObject.GetComponentInChildren<ContentLocalizedLabel>().SetLocalNames(c.ActivityItem.activityLocal);
                    
                    tt.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisposeModule(() =>
                        {
                            _contentModel.AssetDetailsSignalSource = new GetClassificationDetailsCommandSignal(c.ActivityItem.itemId);
                            var assetIds = CreateAssetIdsToDownloadDetails(c.ActivityItem.itemId);
            
                            _signal.Fire(new StartAssetDetailsCommandSignal(assetIds));
                        });
                    });
                });
            }
            
            List<int> CreateAssetIdsToDownloadDetails(int classificationId)
            {
                var activity = _contentModel.GetClassificationById(classificationId).ActivityItem;
                var assetIds = new List<int>();
                foreach (var activityContent in activity.assetContent)
                {
                    assetIds.Add(activityContent.assetId);
                    _contentModel.MultipleAssetDetailsIds.Add(activityContent.assetId);
                }
            
                return assetIds;
            }
        }
        
        private void SetupPuzzlePanel()
        {
            List<ActivityItem> puzzle = _contentModel.GetAssetById(_contentViewModel.mainAssetID).Puzzle;
            List<ClientActivityModel> multiplePuzzle = new List<ClientActivityModel>();
            
            _contentModel.MultiplePuzzle.ForEach(mq =>
            {
                if (mq.ActivityItem.assetContent.FindAll(ac => ac.assetId == _contentViewModel.mainAssetID).Count > 0)
                {
                    multiplePuzzle.Add(mq);
                }
            });

            if (puzzle.Count > 0 || multiplePuzzle.Count > 0)
            {
                _activitiesPanel.puzzlePanel.SetActive(true);
                CreatePuzzleList();
            }
            else
            {
                _activitiesPanel.puzzlePanel.SetActive(false);
            }

            void CreatePuzzleList()
            {
                _activitiesPanel.puzzleTemplate.gameObject.DestroyNeighbors();

                puzzle.ForEach(q =>
                {
                    TextTooltipEvents tt = _activitiesPanel.puzzleTemplate.gameObject.Duplicate<TextTooltipEvents>();
                    tt.gameObject.SetActive(true);
                    tt.textMeshPro.text = GetCurrentTranslationsForItem(q.activityLocal);
                    tt.gameObject.GetComponentInChildren<ContentLocalizedLabel>().SetLocalNames(q.activityLocal);

                    tt.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisposeModule(() =>
                        {
                            _contentModel.AssetDetailsSignalSource = new StartPuzzleCommandSignal(_contentViewModel.mainAssetID, q.itemId);
                            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {_contentViewModel.mainAssetID}));
                        });
                    });
                });
                
                multiplePuzzle.ForEach(q =>
                {
                    TextTooltipEvents tt = _activitiesPanel.puzzleTemplate.gameObject.Duplicate<TextTooltipEvents>();
                    tt.gameObject.SetActive(true);
                    tt.textMeshPro.text = GetCurrentTranslationsForItem(q.ActivityItem.activityLocal);
                    tt.gameObject.GetComponentInChildren<ContentLocalizedLabel>().SetLocalNames(q.ActivityItem.activityLocal);

                    tt.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisposeModule(() =>
                        {
                            _contentModel.AssetDetailsSignalSource = new StartMultiplePuzzleCommandSignal(q.ActivityItem.itemId);
                            List<int> ids = CreateAssetIdsToDownloadDetails(q.ActivityItem.itemId);
                            
                            _signal.Fire(new StartAssetDetailsCommandSignal(ids)); 
                        });
                    });
                });
            }
            
            List<int> CreateAssetIdsToDownloadDetails(int activityId)
            {
                var activity = _contentModel.GetMultiplePuzzleById(activityId).ActivityItem;
                var assetIds = new List<int>();
                foreach (var activityContent in activity.assetContent)
                {
                    assetIds.Add(activityContent.assetId);
                    _contentModel.MultipleAssetDetailsIds.Add(activityContent.assetId);
                }
            
                return assetIds;
            }
        }

        private void SetupQuizPanel()
        {
            List<ActivityItem> quiz = _contentModel.GetAssetById(_contentViewModel.mainAssetID).Quiz;
            List<ClientActivityModel> multipleQuiz = new List<ClientActivityModel>();
            
            _contentModel.MultipleQuiz.ForEach(mq =>
            {
                if (mq.ActivityItem.assetContent.FindAll(ac => ac.assetId == _contentViewModel.mainAssetID).Count > 0)
                {
                    multipleQuiz.Add(mq);
                }
            });

            if (quiz.Count > 0 || multipleQuiz.Count > 0)
            {
                _activitiesPanel.quizPanel.SetActive(true);
                CreateQuizList();
            }
            else
            {
                _activitiesPanel.quizPanel.SetActive(false);
            }

            void CreateQuizList()
            {
                _activitiesPanel.quizTemplate.gameObject.DestroyNeighbors();
                
                quiz.ForEach(q =>
                {
                    TextTooltipEvents tt = _activitiesPanel.quizTemplate.gameObject.Duplicate<TextTooltipEvents>();
                    tt.gameObject.SetActive(true);
                    tt.textMeshPro.text = GetCurrentTranslationsForItem(q.activityLocal);
                    tt.gameObject.GetComponentInChildren<ContentLocalizedLabel>().SetLocalNames(q.activityLocal);

                    tt.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisposeModule(() =>
                        {
                            _contentModel.AssetDetailsSignalSource = new StartQuizCommandSignal(_contentViewModel.mainAssetID, q.itemId);
                            _signal.Fire(new StartAssetDetailsCommandSignal(new List<int> {_contentViewModel.mainAssetID}));
                        });
                    });
                });
                
                multipleQuiz.ForEach(q =>
                {
                    TextTooltipEvents tt = _activitiesPanel.quizTemplate.gameObject.Duplicate<TextTooltipEvents>();
                    tt.gameObject.SetActive(true);
                    tt.textMeshPro.text = GetCurrentTranslationsForItem(q.ActivityItem.activityLocal);
                    tt.gameObject.GetComponentInChildren<ContentLocalizedLabel>().SetLocalNames(q.ActivityItem.activityLocal);

                    tt.gameObject.GetComponentInChildren<Button>().onClick.AddListener(() =>
                    {
                        DisposeModule(() =>
                        {
                            _contentModel.AssetDetailsSignalSource = new StartMultipleQuizCommandSignal(q.ActivityItem.itemId);
                            List<int> ids = CreateAssetIdsToDownloadDetails(q.ActivityItem.itemId);
                            
                            _signal.Fire(new StartAssetDetailsCommandSignal(ids)); 
                        });
                    });
                });
            }
            
            List<int> CreateAssetIdsToDownloadDetails(int activityId)
            {
                var activity = _contentModel.GetMultipleQuizById(activityId).ActivityItem;
                var assetIds = new List<int>();
                foreach (var activityContent in activity.assetContent)
                {
                    assetIds.Add(activityContent.assetId);
                    _contentModel.MultipleAssetDetailsIds.Add(activityContent.assetId);
                }
            
                return assetIds;
            }
        }
        #endregion

        #region Related
        private void SetupRelatedView()
        {
            SetupModelRelatedPanel();
            SetupLabelRelatedPanel();
            
            if(!_relatedPanel.modelRelatedPanel.activeSelf && !_relatedPanel.labelRelatedPanel.activeSelf)
                _view.relatedButton.gameObject.SetActive(false);
            else
                _view.relatedButton.gameObject.SetActive(true);
        }
        
        private void SetupLabelRelatedPanel()
        {
            if (!_contentModel.HasAssetLabels(_contentViewModel.mainAssetID))
            {
                _relatedPanel.labelRelatedPanel.SetActive(false);
                return;
            }
            
            List<(string label, LocalName[] localNames, int assetId, string assetType, bool hasPlusButton)> hotSpotList = new List<(string label, LocalName[] localNames, int assetId, string assetType, bool hasPlusButton)>();
            
            foreach (var v in _contentModel.GetAssetById(_contentViewModel.mainAssetID).AssetDetail.AssetContentPlatform.assetLabel)
            {
                if (v.labelHotSpot != null && v.labelHotSpot.Length > 0)
                {
                    foreach (var associatedAsset in v.labelHotSpot)
                    {
                        string labelName = GetAssociatedContentTranslation(associatedAsset.AssetId,
                            _contentViewModel.CurrentContentViewCultureCode);
                        
                        string type = associatedAsset.Type.ToLower();
                        bool hasPlus = !type.Equals(AssetTypeConstants.Type_2D_Video) &&
                                       !type.Equals(AssetTypeConstants.Type_3D_Video);

                        var localizedName = _contentModel.GetAssetById(associatedAsset.AssetId)?.LocalizedName;

                        List<LocalName> ln = new List<LocalName>();

                        localizedName?.Keys.ToList().ForEach(k =>
                        {
                            ln.Add(new LocalName() {Culture = k, Name = localizedName[k]});
                        });

                        if(hotSpotList.Find(hid => hid.assetId == associatedAsset.AssetId).assetId != associatedAsset.AssetId && localizedName != null)
                            hotSpotList.Add((labelName, ln.ToArray(), associatedAsset.AssetId, type, hasPlus));
                    }
                }
            }
            
            if (hotSpotList == null || hotSpotList.Count == 0)
            {
                _relatedPanel.labelRelatedPanel.SetActive(false);
            }
            else
            {
                _relatedPanel.labelRelatedPanel.SetActive(true);
                CreateLabelRelatedList();
            }
           
            void CreateLabelRelatedList()
            {
                _relatedPanel.labelRelatedTemplate.gameObject.DestroyNeighbors();

                hotSpotList.ForEach(hs =>
                {
                    TextTooltipEvents tt = _relatedPanel.labelRelatedTemplate.gameObject.Duplicate<TextTooltipEvents>();
                    tt.gameObject.SetActive(true);
                    tt.textMeshPro.text = hs.label;
                    
                    tt.gameObject.GetComponentInChildren<ContentLocalizedLabel>().SetLocalNames(hs.localNames);

                    Button btn = tt.transform.Get<Button>("Item_Button");
                    
                    if (hs.assetType.Equals(AssetTypeConstants.Type_2D_Video)
                        || hs.assetType.Equals(AssetTypeConstants.Type_3D_Video))
                    {
                        btn.onClick.AddListener(() => _contentViewMediator.OnAssociatedContentClick(hs.assetId));
                    }
                    else
                    {
                        btn.onClick.AddListener(() =>
                        {
                            ExploreMode();
                            _contentViewMediator.OpenNewAsset(hs.assetId);
                        });

                        if (hs.hasPlusButton)
                        {
                            Button plusBtn = tt.transform.Get<Button>("X_Plus_Button");
                            plusBtn.gameObject.SetActive(true);
                            plusBtn.onClick.AddListener(() =>
                            {
                                ExploreMode();
                                _contentViewMediator.StartMultimodelMode(hs.assetId);
                            });
                        }
                    }
                });
            }
        }

        private void SetupModelRelatedPanel()
        {
            AssociatedAsset[] associatedContent = _contentModel.GetAssetById(_contentViewModel.mainAssetID).AssetDetail.AssociatedContents;
            
            if (associatedContent == null || associatedContent.Length == 0)
            {
                _relatedPanel.modelRelatedPanel.SetActive(false);
            }
            else
            {
                _relatedPanel.modelRelatedPanel.SetActive(true);
                CreateModelRelatedList();
            }
           
            void CreateModelRelatedList()
            {
                _relatedPanel.modelRelatedTemplate.gameObject.DestroyNeighbors();

                associatedContent.ToList().ForEach(ac =>
                {
                    ClientAssetModel data = _contentModel.GetAssetById(ac.AssetId);

                    if (data != null)
                    {
                        TextTooltipEvents tt = _relatedPanel.modelRelatedTemplate.gameObject.Duplicate<TextTooltipEvents>();
                        tt.gameObject.SetActive(true);
                        tt.textMeshPro.text = GetAssociatedContentTranslation(data.LocalizedName, _contentViewModel.CurrentContentViewCultureCode);
                        
                        List<LocalName> ln = new List<LocalName>();
                        
                        data.LocalizedName.Keys.ToList().ForEach(k =>
                        {
                            ln.Add(new LocalName() {Culture = k, Name = data.LocalizedName[k]});
                        });
                        
                        tt.gameObject.GetComponentInChildren<ContentLocalizedLabel>().SetLocalNames(ln.ToArray());

                        Button btn = tt.transform.Get<Button>("Item_Button");
                        
                        if (ac.Type.ToLower().Equals(AssetTypeConstants.Type_2D_Video)
                            || ac.Type.ToLower().Equals(AssetTypeConstants.Type_3D_Video))
                        {
                            btn.onClick.AddListener(() => _contentViewMediator.OnAssociatedContentClick(data.Asset.Id));
                        }
                        else
                        {
                            btn.onClick.AddListener(() =>
                            {
                                ExploreMode();
                                _contentViewMediator.OpenNewAsset(data.Asset.Id);
                            });

                            if (ac.Type.ToLower().Equals(AssetTypeConstants.Type_3D_Model))
                            {
                                Button plusBtn = tt.transform.Get<Button>("X_Plus_Button");
                                plusBtn.gameObject.SetActive(true);
                                plusBtn.onClick.AddListener(() =>
                                {
                                    ExploreMode();
                                    _contentViewMediator.StartMultimodelMode(data.Asset.Id);
                                });
                            }
                        }
                    }
                });
            }
        }

        #endregion

        #region Home

        private void SetupHomeView()
        {
            _contentViewModel.contentViewPC.homeButton.onClick.AddListener(ShowHome);
            
            _view.closeButton.onClick.AddListener(() => DisposeModule());
            _view.exploreButton.onClick.AddListener(ExploreMode);
            _view.explainButton.onClick.AddListener(ExplainMode);
            
            _view.activitiesButton.onClick.AddListener(() => ActivitiesMode(true));
            _activitiesPanel.backButton.onClick.AddListener(() => ActivitiesMode(false));
            
            _view.relatedButton.onClick.AddListener(() => RelatedMode(true));
            _relatedPanel.backButton.onClick.AddListener(() => RelatedMode(false));
            
            _asyncProcessor.Wait(0.2f, () =>
            {
                _contentViewModel.contentViewPC.smoothOrbitCam.interactable = false;
            });
        }
        
        private void ShowHome()
        {
            SetAllLabelActive(false);
            _signal.Fire<CloseDescriptionViewSignal>();
            _signal.Fire(new ContentPaint3DStateChangedSignal(false, true));
            _view.gameObject.SetActive(true);
            _view.homePanel.SetActive(true);
            _activitiesPanel.SetActive(false);
            _relatedPanel.SetActive(false);
            _contentViewModel.contentViewPC.gameObject.SetActive(false);
            CameraResetBase(_contentViewModel.contentViewPC.smoothOrbitCam, selfTarget: IsAsset360Model(_contentViewModel.mainAssetID));
            _asyncProcessor.Wait(0.3f, () =>
            {
                _contentViewModel.contentViewPC.smoothOrbitCam.interactable = false;
            });
            UpdateLocalization();
            ShowContentHomeScreenAction?.Invoke(true);
        }
        #endregion

        #region Modes

        private void ActivitiesMode(bool isOn)
        {
            _view.homePanel.SetActive(!isOn);
            _activitiesPanel.SetActive(isOn);
        }
        
        private void RelatedMode(bool isOn)
        {
            _view.homePanel.SetActive(!isOn);
            _relatedPanel.SetActive(isOn);
        }

        private void ExplainMode()
        {
            ExploreMode();
            
            _asyncProcessor.Wait(0, () =>
            {
                _contentViewModel.contentViewPC.recorderToggle.isOn = true;
            });
        }

        private void ExploreMode()
        {
            _view.gameObject.SetActive(false);
            _activitiesPanel.SetActive(false);
            _relatedPanel.SetActive(false);
            _contentViewModel.contentViewPC.gameObject.SetActive(true);
            _contentViewModel.contentViewPC.smoothOrbitCam.interactable = true;

            _signal.Fire(new StartAugmentedRealitySignal(isInit =>
            {
                _isOnArMode = isInit;
                InitializeARAction?.Invoke(isInit);

                if (isInit)
                {
                    _contentViewModel.contentViewPC.paintView.gameObject.SetActive(false);
                    _contentViewModel.contentViewPC.multipartButton.gameObject.SetActive(false);
                }
            }));
        }
        
        #endregion
        
        public void Dispose()
        {
            InitializeModuleAction -= InitializeModule;
            InitializeARAction -= InitializeAR;
        }

        public void OnViewEnable()
        {
        }

        public void OnViewDisable()
        {
        }
    }
}