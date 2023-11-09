using System;
using System.Collections.Generic;
using System.Linq;
using TDL.Core;
using TDL.Server;
using UnityEngine;
using Zenject;

namespace TDL.Models
{
    public class ContentModel
    {
        public Action OnContentModelChanged;
        public Action OnContentModelChangedAsGuest;
        public Action OnGradeChanged;
        public Action OnSubjectChanged;
        public Action OnTopicChanged;
        public Action OnSubtopicChanged;
        public Action OnSearchChanged;
        public Action OnActivityChanged;
        public Action OnChosenActivityChanged;
        public Action<bool> OnCategoryWithHeadersChanged;

        [Inject] private UserLoginModel _userLoginModel;

        public ISignal AssetDetailsSignalSource { get; set; }
        public List<int> MultipleAssetDetailsIds = new List<int>();
        public List<ClientAssetModel> FavoriteAssets { get; private set; }
        public List<ClientAssetModel> RecentAssets { get; private set; }

        public List<ClientActivityModel> MultipleQuiz { get; set; }
        public List<ClientActivityModel> MultiplePuzzle { get; set; }
        public List<ClientActivityModel> Classifications { get; set; }
        public List<ActivityDetail> ActivityDetails { get; set; } = new List<ActivityDetail>();
        private List<ClientGradeModel> _grades { get; set; }
        private Dictionary<int, ClientAssetModel> _allAssets { get; set; }
        private Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> AllAssetLocalDesc { get; set; }
        private Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> AllAssetLocalStudentDesc { get; set; }
        
        public ClientGradeModel PreviousGrade { get; set; }
        public ClientGradeModel lastSelectedGrade;
        private ClientGradeModel _selectedGrade;
        
        // cached arrays
        public List<Texture2D> CachedTextures = new List<Texture2D>();
        private List<ClientAssetModel> _cachedFoundedAssets = new List<ClientAssetModel>();
        private Dictionary<int, ClientSubjectModel> _cachedFoundedSubjectsById = new Dictionary<int, ClientSubjectModel>();
        private List<ClientActivityModel> _cachedMultipleQuiz = new List<ClientActivityModel>();
        private List<ClientActivityModel> _cachedMultiplePuzzle = new List<ClientActivityModel>();
        private List<ClientActivityModel> _cachedMultipleClassifications = new List<ClientActivityModel>();
        
        public ICategory SelectedCategory { get; private set; }
        
        public enum CategoryNames
        {
            GradeId,
            SubjectId,
            TopicId,
            SubtopicId
        }
        
        private Dictionary<CategoryNames, int> _currentCategoryPath = new Dictionary<CategoryNames, int>();
        public Dictionary<CategoryNames, int> CurrentCategoryPath
        {
            get => _currentCategoryPath;
            private set => _currentCategoryPath = value;
        }
        
        private void UpdateCategoryPath()
        {
            switch (SelectedCategory)
            {
                case ClientSubjectModel subject:

                    _currentCategoryPath[CategoryNames.GradeId] = _selectedGrade.Id;
                    _currentCategoryPath[CategoryNames.SubjectId] = _selectedSubject.Id;
                    _currentCategoryPath[CategoryNames.TopicId] = 0;
                    _currentCategoryPath[CategoryNames.SubtopicId] = 0;
                    
                    break;
                
                case ClientTopicModel topic:
                    
                    _currentCategoryPath[CategoryNames.GradeId] = _selectedGrade.Id;
                    _currentCategoryPath[CategoryNames.SubjectId] = _selectedSubject.Id;
                    _currentCategoryPath[CategoryNames.TopicId] = _selectedTopic.Id;
                    _currentCategoryPath[CategoryNames.SubtopicId] = 0;
                    
                    break;
                
                case ClientSubtopicModel subtopic:
                    
                    _currentCategoryPath[CategoryNames.GradeId] = _selectedGrade.Id;
                    _currentCategoryPath[CategoryNames.SubjectId] = _selectedSubject.Id;
                    _currentCategoryPath[CategoryNames.TopicId] = _selectedTopic.Id;
                    _currentCategoryPath[CategoryNames.SubtopicId] = _selectedSubtopic.Id;
                    
                    break;
            }
        }

        public ClientGradeModel SelectedGrade
        {
            get { return _selectedGrade; }
            set
            {
                if (SelectedGrade == value) return;
                _selectedGrade = value;
                SelectedGrade = value;
                
                if (value != null)
                    OnGradeChanged?.Invoke();
            }
        }
        
        public ClientSubjectModel lastSelectedSubject;
        private ClientSubjectModel _selectedSubject;

        public ClientSubjectModel SelectedSubject
        {
            get { return _selectedSubject; }
            set
            {
                if (SelectedCategory == value) return;
                _selectedSubject = value;
                SelectedCategory = value;
                
                UpdateCategoryPath();
                
                if (value != null)
                    OnSubjectChanged?.Invoke();
            }
        }

        public ClientTopicModel lastSelectedTopic;
        private ClientTopicModel _selectedTopic;

        public ClientTopicModel SelectedTopic
        {
            get { return _selectedTopic; }
            set
            {
                if (SelectedCategory == value) return;
                _selectedTopic = value;
                SelectedCategory = value;
                
                UpdateCategoryPath();
                
                if (value != null)
                    OnTopicChanged?.Invoke();
            }
        }

        public ClientSubtopicModel lastSelectedSubtopic;
        private ClientSubtopicModel _selectedSubtopic;

        public ClientSubtopicModel SelectedSubtopic
        {
            get { return _selectedSubtopic; }
            set
            {
                if (SelectedCategory == value) return;
                _selectedSubtopic = value;
                SelectedCategory = value;
                
                UpdateCategoryPath();

                if (value != null)
                    OnSubtopicChanged?.Invoke();
                
            }
        }

        private ClientAssetModel _selectedAsset;

        public ClientAssetModel SelectedAsset
        {
            get { return _selectedAsset; }
            set
            {
                if (value == null || _selectedAsset == value) return;
                _selectedAsset = value;
            }
        }

        private List<ClientAssetModel> _searchResult = new List<ClientAssetModel>();

        public List<ClientAssetModel> SearchResult
        {
            get { return _searchResult; }
            set
            {
                if (value == null || _searchResult == value) return;
                _searchResult = value;
                OnSearchChanged?.Invoke();
            }
        }

        private ClientActivityModel _selectedActivity;
        public ClientActivityModel SelectedActivity
        {
            get { return _selectedActivity; }
            set
            {
                if (value == null || SelectedCategory == value) return;
                _selectedActivity = value;
                SelectedCategory = value;
                OnActivityChanged?.Invoke();
            }
        }

        private ClientChosenActivityModel _selectedChosenActivity = new ClientChosenActivityModel();
        public ClientChosenActivityModel SelectedChosenActivity
        {
            get { return _selectedChosenActivity; }
        }

        public void ForceUpdateChosenActivity()
        {
            OnChosenActivityChanged?.Invoke();
        }

        private bool _isCategoryWithHeadersActive;
        public bool IsCategoryWithHeadersActive
        {
            get { return _isCategoryWithHeadersActive; }
            set
            {
                if (_isCategoryWithHeadersActive == value) return;
                _isCategoryWithHeadersActive = value;
                OnCategoryWithHeadersChanged?.Invoke(_isCategoryWithHeadersActive);
            }
        }

        public void UpdateMainContent(ContentModelStruct contentModelStruct)
        {
            RecentAssets = contentModelStruct.RecentAssets;
            FavoriteAssets = contentModelStruct.FavoriteAssets;
            MultipleQuiz = contentModelStruct.MultipleQuizzes;
            MultiplePuzzle = contentModelStruct.MultiplePuzzles;
            Classifications = contentModelStruct.Classifications;
            _grades = contentModelStruct.Grades;
            _allAssets = contentModelStruct.AllAssets;
            AllAssetLocalDesc = contentModelStruct.AllAssetLocalDesc;
            AllAssetLocalStudentDesc = contentModelStruct.AllAssetLocalStudentDesc;
            
            NotifyUpdated();
        }
        
        public void NotifyUpdated()
        {
            if (_userLoginModel.IsLoggedAsUser)
            {
                OnContentModelChanged?.Invoke();
                return;
            }
            
            OnContentModelChangedAsGuest?.Invoke();
        }

        public List<ClientGradeModel> GetGrades()
        {
            return _grades.OrderBy(grade => grade.Grade.name).ToList();
        }

        public List<ClientSubjectModel> GetSubjects(ClientGradeModel grade)
        {
            return grade.ClientSubjects.OrderBy(subject => subject.Subject.name).ToList();
        }

        public List<ClientSubjectModel> GetAllSubjects()
        {
            List<ClientSubjectModel> allSubjects = new List<ClientSubjectModel>();

            foreach (ClientGradeModel clientGradeModel in GetGrades())
            {
                allSubjects.AddRange(GetSubjects(clientGradeModel));
            }

            return allSubjects;
        }

        public List<ClientTopicModel> GetTopics()
        {
            return SelectedSubject.ClientTopics.OrderBy(topic => topic.Topic.name).ToList();
        }

        public List<ClientSubtopicModel> GetSubtopics()
        {
            return SelectedTopic.ClientSubtopics.OrderBy(subtopic => subtopic.Subtopic.name).ToList();
        }

        public ClientGradeModel GetGradeById(int gradeId)
        {
            return GetGrades().FirstOrDefault(item => item.Grade.id == gradeId);
        }

        public ClientSubjectModel GetSubjectById(int subjectId)
        {
            return SelectedGrade.ClientSubjects.FirstOrDefault(item => item.Subject.id == subjectId);
        }
        
        public ClientSubjectModel GetClientSubjectById(int subjectId)
        {
            _cachedFoundedSubjectsById.Clear();
            
            foreach (var clientGradeModel in GetGrades())
            {
                foreach (ClientSubjectModel clientSubjectModel in GetSubjects(clientGradeModel))
                {
                    if (!_cachedFoundedSubjectsById.ContainsKey(clientSubjectModel.Subject.id))
                        _cachedFoundedSubjectsById.Add(clientSubjectModel.Subject.id, clientSubjectModel);
                }
            }

            return _cachedFoundedSubjectsById[subjectId];
        }

        public ClientTopicModel GetTopicById(int topicId)
        {
            return _selectedSubject.ClientTopics.FirstOrDefault(item => item.Topic.id == topicId);
        }

        public ClientSubtopicModel GetSubtopicById(int subtopicId)
        {
            return _selectedTopic.ClientSubtopics.FirstOrDefault(item => item.Subtopic.id == subtopicId);
        }

        public List<AssetLocalDesc> GetCurrentAssetLocalDesc(int assetId, bool returnOnlyStudentDescription = false)
        {
            int gradeId = SelectedGrade?.Id ?? -1;
            
            return GetAssetLocalDesc(gradeId, assetId, returnOnlyStudentDescription);
        }
        
        private List<AssetLocalDesc> GetAssetLocalDesc(int gradeId, int assetId, bool returnOnlyStudentDescription)
        {
            if (returnOnlyStudentDescription)
            {
                if (AllAssetLocalStudentDesc.ContainsKey((gradeId, assetId)))
                {
                    return AllAssetLocalStudentDesc[(gradeId, assetId)].ToList();
                }
            }
            else
            {
                if (AllAssetLocalDesc.ContainsKey((gradeId, assetId)))
                {
                    return AllAssetLocalDesc[(gradeId, assetId)].ToList();
                }
                
                if (gradeId < 0)
                    return _allAssets.ContainsKey(assetId) ? _allAssets[assetId]?.Asset?.LocalizedDescription?.ToList() : null;
            }

            return null;
        }

        public List<ClientAssetModel> GetAssetsById(int assetId)
        {
            return _allAssets.Where(x => x.Value.Asset.Id == assetId)
                .Select(x => x.Value).ToList();
        }

        public AssetLocalDesc GetLocalDesc(int assetId)
        {
            return AllAssetLocalDesc.Where(x => x.Key.assetId == 985)
                .FirstOrDefault(x => x.Key.gradeId == 43).Value[0];
        }

        public ClientAssetModel GetAssetById(int assetId)
        {
            return _allAssets.ContainsKey(assetId) ? _allAssets[assetId] : null;
        }
        
        public List<ClientAssetModel> GetAssetsByIds(int[] assetIds)
        {
            _cachedFoundedAssets.Clear();

            foreach (int assetId in assetIds)
            {
                var asset = GetAssetById(assetId);
                
                if(asset != null)
                    _cachedFoundedAssets.Add(asset);
            }
            
            return _cachedFoundedAssets;
        }
        
        public Dictionary<string, string> GetAssetLocalizedName(int assetId)
        {
            return GetAssetById(assetId).Asset.AssetLocal.ToDictionary(keyData => keyData.Culture, valueData => valueData.Name);
        }
        
        public bool HasAssetById(int assetId)
        {
            return _allAssets.ContainsKey(assetId);
        }

        public List<ClientAssetModel> GetAssetsInSelectedCategory(string currentLanguage, string fallbackLanguage)
        {
             var v = SelectedCategory.AssetsInCategory.OrderBy(asset => asset.LocalizedName.ContainsKey(currentLanguage) 
                ? asset.LocalizedName[currentLanguage]
                : asset.LocalizedName[fallbackLanguage]).ToList();

             v.ToList().ForEach(l => l.Asset.GradeId = SelectedGrade.Id);
             return v;
        }

        public List<ClientAssetModel> GetFavoriteAssetsAlphabetical(string currentLanguage, string fallbackLanguage)
        {
            return FavoriteAssets.OrderBy(asset => asset.LocalizedName.ContainsKey(currentLanguage) 
                ? asset.LocalizedName[currentLanguage]
                : asset.LocalizedName[fallbackLanguage]).ToList();
        }

        public List<ClientAssetModel> GetAllAvailableAssets(uint max = 0)
        {
            if (max == 0)
            {
                return _allAssets.Values.ToList();
            }
            
            var list = _allAssets.Values.ToList();
            return list.Take((int)max).ToList();
        }

        public List<ClientAssetModel> GetRecentAssets(int maxNumber)
        {
            RecentAssets.Sort((x, y) => DateTime.Compare(y.Asset.RecentDate, x.Asset.RecentDate));
            return RecentAssets.Take(maxNumber).ToList();
        }

        public int GetRecentAssetsCount()
        {
            return _allAssets.Values.Count(asset => asset.IsRecentlyViewed);
        }

        public bool HasAssetLabels(int assetID)
        {
            var assetLabelContents = GetAssetById(assetID).AssetDetail.AssetContentPlatform;

            if (assetLabelContents == null)
            {
                return false;
            }

            var assetLabel = assetLabelContents.assetLabel;

            return assetLabel != null && assetLabel.Length != 0;
        }

        public bool HasDescription(int assetID, string cultureCode, bool isOnlyStudentDescription = false, int gradeID = -1)
        {
            List<AssetLocalDesc> assetDesc;
            if (gradeID < 0)
            {
                assetDesc = GetCurrentAssetLocalDesc(assetID, isOnlyStudentDescription);
            }
            else
            {
                assetDesc = GetAssetLocalDesc(gradeID, assetID, isOnlyStudentDescription);
            }
            
            if (assetDesc == null || assetDesc.Count == 0)
                return false;

            return assetDesc.Any(item => item.Culture.Equals(cultureCode));
        }

        public bool HasBackground(int id)
        {
            var assetContents = GetAssetById(id).AssetDetail.AssetContentPlatform;

            if (assetContents == null)
            {
                return false;
            }

            return !string.IsNullOrEmpty(assetContents.BackgroundUrl);
        }

        public bool HasCategoryOnlyOwnAssets()
        {
            return SelectedCategory.AssetsInCategory.Count > 0 && SelectedCategory.AssetsInChildren.Count == 0;
        }
        
        public bool HasCategoryAnyActivity()
        {
            return HasCategoryAnyQuiz() || HasCategoryAnyPuzzle() || HasCategoryAnyMultipleQuiz() ||
                   HasCategoryAnyMultiplePuzzle() || HasCategoryAnyClassifications();
        }

        public bool HasCategoryAnyQuiz()
        {
            if (SelectedCategory.AllAssets.Count > 0)
            {
                return SelectedCategory.AllAssets.Any(item => item.Quiz.Count > 0);
            }

            return false;
        }

        public bool HasCategoryAnyPuzzle()
        {
            if (SelectedCategory.AllAssets.Count > 0)
            {
                return SelectedCategory.AllAssets.Any(item => item.Puzzle.Count > 0);
            }

            return false;
        }

        public bool HasCategoryAnyMultipleQuiz()
        {
            return HasCategoryAnyMultipleActivity(MultipleQuiz);
        }

        public bool HasCategoryAnyMultiplePuzzle()
        {
            return HasCategoryAnyMultipleActivity(MultiplePuzzle);
        }

        public bool HasCategoryAnyClassifications()
        {
            return HasCategoryAnyMultipleActivity(Classifications);
        }

        private bool HasCategoryAnyMultipleActivity(List<ClientActivityModel> activityModel)
        {
            foreach (var classification in activityModel)
            {
                foreach (var categoryPath in classification.DirectPathToActivity)
                {
                    if (_currentCategoryPath[CategoryNames.TopicId] > 0 
                        && _currentCategoryPath[CategoryNames.SubtopicId] > 0)
                    {
                        if (categoryPath[CategoryNames.GradeId] == _currentCategoryPath[CategoryNames.GradeId]
                            && categoryPath[CategoryNames.SubjectId] == _currentCategoryPath[CategoryNames.SubjectId]
                            && categoryPath[CategoryNames.TopicId] == _currentCategoryPath[CategoryNames.TopicId]
                            && categoryPath[CategoryNames.SubtopicId] == _currentCategoryPath[CategoryNames.SubtopicId])
                        {
                            return true;
                        }
                    }
                    else if (_currentCategoryPath[CategoryNames.TopicId] > 0)
                    {
                        if (categoryPath[CategoryNames.GradeId] == _currentCategoryPath[CategoryNames.GradeId]
                            && categoryPath[CategoryNames.SubjectId] == _currentCategoryPath[CategoryNames.SubjectId]
                            && categoryPath[CategoryNames.TopicId] == _currentCategoryPath[CategoryNames.TopicId])
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (categoryPath[CategoryNames.GradeId] == _currentCategoryPath[CategoryNames.GradeId]
                            && categoryPath[CategoryNames.SubjectId] == _currentCategoryPath[CategoryNames.SubjectId])
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public List<ClientAssetModel> GetQuizzesInCurrentCategory(string currentLanguage, string fallbackLanguage)
        {
            return SelectedCategory.AllAssets.Where(asset => asset.Quiz.Count > 0)
                .OrderBy(item => item.LocalizedName.ContainsKey(currentLanguage)
                    ? item.LocalizedName[currentLanguage]
                    : item.LocalizedName[fallbackLanguage])
                .GroupBy(item => item.LocalizedName.ContainsKey(currentLanguage)
                    ? item.LocalizedName[currentLanguage]
                    : item.LocalizedName[fallbackLanguage])
                .Select(asset => asset.First())
                .ToList();
        }

        public List<ClientAssetModel> GetPuzzlesInCurrentCategory(string currentLanguage, string fallbackLanguage)
        {
            return SelectedCategory.AllAssets.Where(asset => asset.Puzzle.Count > 0)
                .OrderBy(item => item.LocalizedName.ContainsKey(currentLanguage)
                    ? item.LocalizedName[currentLanguage]
                    : item.LocalizedName[fallbackLanguage])
                .GroupBy(item => item.LocalizedName.ContainsKey(currentLanguage)
                    ? item.LocalizedName[currentLanguage]
                    : item.LocalizedName[fallbackLanguage])
                .Select(asset => asset.First())
                .ToList();
        }

        public List<ClientActivityModel> GetMultipleQuizInCurrentCategory()
        {
            return GetMultipleActivityInCurrentCategory(MultipleQuiz, _cachedMultipleQuiz);
        }

        public List<ClientActivityModel> GetMultiplePuzzleInCurrentCategory()
        {
            return GetMultipleActivityInCurrentCategory(MultiplePuzzle, _cachedMultiplePuzzle);
        }

        public List<ClientActivityModel> GetClassificationsInCurrentCategory()
        {
            return GetMultipleActivityInCurrentCategory(Classifications, _cachedMultipleClassifications);
        }

        private List<ClientActivityModel> GetMultipleActivityInCurrentCategory(List<ClientActivityModel> activityModels, List<ClientActivityModel> cachedActivityModels)
        {
            cachedActivityModels.Clear();
            
            if (activityModels != null)
            {
                foreach (var activity in activityModels)
                {
                    foreach (var categoryPath in activity.DirectPathToActivity)
                    {
                        if (_currentCategoryPath[CategoryNames.TopicId] > 0 
                            && _currentCategoryPath[CategoryNames.SubtopicId] > 0)
                        {
                            if (categoryPath[CategoryNames.GradeId] == _currentCategoryPath[CategoryNames.GradeId]
                                && categoryPath[CategoryNames.SubjectId] == _currentCategoryPath[CategoryNames.SubjectId]
                                && categoryPath[CategoryNames.TopicId] == _currentCategoryPath[CategoryNames.TopicId]
                                && categoryPath[CategoryNames.SubtopicId] == _currentCategoryPath[CategoryNames.SubtopicId])
                            {
                                if (!cachedActivityModels.Contains(activity))
                                {
                                    cachedActivityModels.Add(activity);
                                }
                            }
                        }
                        else if (_currentCategoryPath[CategoryNames.TopicId] > 0)
                        {
                            if (categoryPath[CategoryNames.GradeId] == _currentCategoryPath[CategoryNames.GradeId]
                                && categoryPath[CategoryNames.SubjectId] == _currentCategoryPath[CategoryNames.SubjectId]
                                && categoryPath[CategoryNames.TopicId] == _currentCategoryPath[CategoryNames.TopicId])
                            {
                                if (!cachedActivityModels.Contains(activity))
                                {
                                    cachedActivityModels.Add(activity);
                                }
                            }
                        }
                        else
                        {
                            if (categoryPath[CategoryNames.GradeId] == _currentCategoryPath[CategoryNames.GradeId]
                                && categoryPath[CategoryNames.SubjectId] == _currentCategoryPath[CategoryNames.SubjectId])
                            {
                                if (!cachedActivityModels.Contains(activity))
                                {
                                    cachedActivityModels.Add(activity);
                                }
                            }
                        }
                    }
                }   
            }

            return cachedActivityModels;
        }

        public ClientActivityModel GetMultipleQuizById(int id)
        {
            return MultipleQuiz.FirstOrDefault(item => item.ActivityItem.itemId == id);
        }
        
        // public ClientActivityModel GetMultipleQuizById(int id)
        // {
        //     return MultipleQuiz.FirstOrDefault(item => item.ActivityItem.itemId == id);
        // }

        public ClientActivityModel GetMultiplePuzzleById(int id)
        {
            return MultiplePuzzle.FirstOrDefault(item => item.ActivityItem.itemId == id);
        }

        public ClientActivityModel GetClassificationById(int id)
        {
            return Classifications.FirstOrDefault(item => item.ActivityItem.itemId == id);
        }

        public List<ClientAssetModel> GetSearchAssetsByType(int[] foundedIds, string[] assetTypes)
        {
            List<ClientAssetModel> list = new List<ClientAssetModel>();
            
            foreach (var assetType in assetTypes)
            {
                list.AddRange(GetSearchAssets(foundedIds)
                    .FindAll(r => r.Asset.Type.ToLower() == assetType));
            }
         
            return list;
        }

        public List<ClientAssetModel> GetSearchAssets(int[] foundedIds)
        {
            _cachedFoundedAssets.Clear();

            foreach (var foundedId in foundedIds)
            {
                if (_allAssets.ContainsKey(foundedId))
                {
                    _cachedFoundedAssets.Add(_allAssets[foundedId]);
                }
            }

            return _cachedFoundedAssets;
        }

        public class ContentModelStruct
        {
            public List<ClientAssetModel> RecentAssets;
            public List<ClientAssetModel> FavoriteAssets;
            public List<ClientActivityModel> MultipleQuizzes;
            public List<ClientActivityModel> MultiplePuzzles;
            public List<ClientActivityModel> Classifications;
            public List<ClientGradeModel> Grades;
            public Dictionary<int, ClientAssetModel> AllAssets;
            public Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> AllAssetLocalDesc;
            public Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> AllAssetLocalStudentDesc;
        }
        
        public void ClearCachedTextures()
        {
            foreach (var texture in CachedTextures)
            {
                // ToDo destroy after each frame
                GameObject.Destroy(texture);
            }

            if (CachedTextures.Count > 0)
            {
                CachedTextures.Clear();
            }
        }

        public void Reset()
        {
            ClearSelectedCategories();

            _grades = null;
            _allAssets = null;
            AssetDetailsSignalSource = null;
            MultipleAssetDetailsIds.Clear();
            FavoriteAssets.Clear();
            RecentAssets.Clear();
            _cachedFoundedAssets.Clear();
            _cachedFoundedSubjectsById.Clear();
            _cachedMultipleQuiz.Clear();
            _cachedMultipleQuiz.Clear();
            _cachedMultipleClassifications.Clear();
        }

        // TODO : Check if work it
        public void ClearSelectedCategories()
        {
            SaveLastSelectedCategories();

            SelectedCategory = null;
            SelectedGrade = null;
            PreviousGrade = null;
            _selectedSubject = null;
            _selectedTopic = null;
            _selectedSubtopic = null;
            _selectedAsset = null;
            _selectedActivity = null;
        }

        private void SaveLastSelectedCategories()
        {
            if (_selectedGrade != null)
                lastSelectedGrade = _selectedGrade;

            if (_selectedSubject != null)
                lastSelectedSubject = _selectedSubject;

            if (_selectedTopic != null)
                lastSelectedTopic = _selectedTopic;

            if (_selectedSubtopic != null)
                lastSelectedSubtopic = _selectedSubtopic;
        }
    }
}