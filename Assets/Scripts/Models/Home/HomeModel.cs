using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BestHTTP;
using TDL.Views;
using UnityEngine;

namespace TDL.Models
{
    public class HomeModel
    {
        public Action<bool> OnLeftMenuChanged;
        public Action<bool> OnRightMenuChanged;
        public Action<bool> OnHomeTabFavouritesChanged;
        public Action<bool> OnHomeTabRecentChanged;
        public Action<bool> OnHomeTabMyContentChanged;
        public Action<bool> OnHomeTabMyTeacherChanged;
        public Action<bool> OnHomeTabMyTeacherVisibilityChanged;
        public Action<bool> OnNoSearchResultsChanged;

        public ISignal LastShownCategory;
        public ISignal LastShownActivity;
        
        public List<GradeItemView> ShownGradeOnHome = new List<GradeItemView>();
        public List<SubjectItemView> ShownSubjectOnHome = new List<SubjectItemView>();
        public List<TopicItemView> ShownTopicsOnHome = new List<TopicItemView>();
        public List<SubtopicItemView> ShownSubtopicsOnHome = new List<SubtopicItemView>();
        public List<AssetItemView> ShownAssetsOnHome = new List<AssetItemView>();
        public List<UserContentItemView> ShownUserContentOnHome = new List<UserContentItemView>();
        public Dictionary<string, ViewBase> ShownActivitiesOnHome = new Dictionary<string, ViewBase>();
        public List<QuizAssetView> ShownQuizAssetsOnHome = new List<QuizAssetView>();
        public List<PuzzleAssetItemView> ShownPuzzleAssetsOnHome = new List<PuzzleAssetItemView>();
        public List<MultipleQuizAssetItemView> ShownMultipleQuizAssetsOnHome = new List<MultipleQuizAssetItemView>();
        public List<MultiplePuzzleAssetItemView> ShownMultiplePuzzleAssetsOnHome = new List<MultiplePuzzleAssetItemView>();
        public List<ClassificationAssetItemView> ShownClassificationAssetsOnHome = new List<ClassificationAssetItemView>();
        public Dictionary<string, List<IThumbnailView>> ThumbnailsToDownload = new Dictionary<string, List<IThumbnailView>>();
        public ObservableCollection<DownloadAsset> AssetsToDownload = new ObservableCollection<DownloadAsset>();
        public Dictionary<int, List<int>> MultipleAssetIdQueueToDownload = new Dictionary<int, List<int>>();
        
        public ISelectableMenuItem SelectedMenuItem;
        public Dictionary<string, ISelectableMenuItem> SelectableMenuItems = new Dictionary<string, ISelectableMenuItem>();
        
        public Dictionary<string, DescriptionView> OpenedDescriptions { get; } = new Dictionary<string, DescriptionView>();

        public class DownloadAsset
        {
            public int Id;
            public int Version;
            public string Name;
            public float Progress;
            public HTTPRequest ItemRequest;
        }  

        public RectTransform MainContent { get; set; }
        public Transform LeftMenuContent { get; set; }
        public Transform TopicsSubtopicsContent { get; set; }
        public Transform AssetsContent { get; set; }
        
        public bool IsPopupProgressVisible { get; set; }

        private bool _isLeftMenuActive;
        public bool IsLeftMenuActive
        {
            get { return _isLeftMenuActive; }
            set
            {
                if (_isLeftMenuActive == value) return;
                _isLeftMenuActive = value;
                OnLeftMenuChanged?.Invoke(_isLeftMenuActive);
            }
        }

        private bool _isRightMenuActive;
        public bool IsRightMenuActive
        {
            get { return _isRightMenuActive; }
            set
            {
                if (_isRightMenuActive == value) return;
                _isRightMenuActive = value;
                OnRightMenuChanged?.Invoke(_isRightMenuActive);
            }
        }
        
        private bool _homeTabFavouritesActive;
        public bool HomeTabFavouritesActive
        {
            get { return _homeTabFavouritesActive; }
            set
            {
                if (_homeTabFavouritesActive == value) return;
                _homeTabFavouritesActive = value;
                OnHomeTabFavouritesChanged?.Invoke(_homeTabFavouritesActive);
            }
        }
        
        private bool _homeTabRecentActive;
        public bool HomeTabRecentActive
        {
            get { return _homeTabRecentActive; }
            set
            {
                if (_homeTabRecentActive == value) return;
                _homeTabRecentActive = value;
                OnHomeTabRecentChanged?.Invoke(_homeTabRecentActive);
            }
        }
        
        private bool _homeTabMyContentActive;
        public bool HomeTabMyContentActive
        {
            get { return _homeTabMyContentActive; }
            set
            {
                if (_homeTabMyContentActive == value) return;
                _homeTabMyContentActive = value;
                OnHomeTabMyContentChanged?.Invoke(_homeTabMyContentActive);
            }
        }
        
        private bool _homeTabMyTeacherActive;
        public bool HomeTabMyTeacherActive
        {
            get { return _homeTabMyTeacherActive; }
            set
            {
                if (_homeTabMyTeacherActive == value) return;
                _homeTabMyTeacherActive = value;
                OnHomeTabMyTeacherChanged?.Invoke(_homeTabMyTeacherActive);
            }
        }

        private bool _homeTabMyTeacherVisible;
        public bool HomeTabMyTeacherVisible
        {
            get { return _homeTabMyTeacherVisible; }
            set
            {
                if (_homeTabMyTeacherVisible == value) return;
                _homeTabMyTeacherVisible = value;
                OnHomeTabMyTeacherVisibilityChanged?.Invoke(_homeTabMyTeacherVisible);
            }
        }
        
        private bool _showNoSearchResults;
        public bool ShowNoSearchResults
        {
            get { return _showNoSearchResults; }
            set
            {
                if (_showNoSearchResults == value) return;
                _showNoSearchResults = value;
                OnNoSearchResultsChanged?.Invoke(_showNoSearchResults);
            }
        }

        public void Reset()
        {
            IsLeftMenuActive = false;
            IsRightMenuActive = false;

            HomeTabFavouritesActive = false;
            HomeTabRecentActive = false;
            HomeTabMyContentActive = false;

            SelectedMenuItem = null;
            SelectableMenuItems.Clear();
            ThumbnailsToDownload.Clear();
            AssetsToDownload.Clear();
            MultipleAssetIdQueueToDownload.Clear();
        }
    }
}