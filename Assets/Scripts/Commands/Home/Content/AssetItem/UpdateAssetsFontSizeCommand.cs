using TDL.Constants;
using TDL.Core;
using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class UpdateAssetsFontSizeCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        private int _currentFontSize;
            
        public void Execute()
        {
            if (_accessibilityModel.AssetItemsFontSizeScaler != AccessibilityConstants.FontSizeMedium150)
            {
                _currentFontSize = PlayerPrefsExtension.GetInt(PlayerPrefsKeyConstants.AccessibilityCurrentFontSize);
                
                if (DeviceInfo.IsMobile())
                {
                    UpdateGrades();
                    UpdateSubjects();
                }
                
                UpdateTopics();
                UpdateSubtopics();
                UpdateAssets();
                UpdateActivityItems();
                UpdateQuizAssets();
                UpdatePuzzleAssets();
                UpdateMultipleQuizAssets();
                UpdateMultiplePuzzleAssets();
                UpdateClassificationAssets();
                UpdateUserContents();
            }
        }
        
        private void UpdateGrades()
        {
            foreach (var assetItemView in _homeModel.ShownGradeOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }

        private void UpdateSubjects()
        {
            foreach (var assetItemView in _homeModel.ShownSubjectOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
        
        private void UpdateTopics()
        {
            foreach (var assetItemView in _homeModel.ShownTopicsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }

        private void UpdateSubtopics()
        {
            foreach (var assetItemView in _homeModel.ShownSubtopicsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }

        private void UpdateAssets()
        {
            foreach (var assetItemView in _homeModel.ShownAssetsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                // more dropdown
                var favoriteAdd = assetItemView.MoreFavoriteAdd;
                favoriteAdd.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var favoriteRemove = assetItemView.MoreFavoriteRemove;
                favoriteRemove.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var favoriteTextToSpeech = assetItemView.MoreFavoriteTextToSpeech;
                favoriteTextToSpeech.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var moreDownloadTextToSpeech = assetItemView.MoreDownloadTextToSpeech;
                moreDownloadTextToSpeech.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var moreDownload = assetItemView.MoreDownload;
                moreDownload.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var moreDownloadDelete = assetItemView.MoreDownloadDelete;
                moreDownloadDelete.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var moreDescription = assetItemView.MoreDescription;
                moreDescription.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var studentNotesText = assetItemView.StudentNotesText;
                studentNotesText.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var morePuzzle = assetItemView.MorePuzzle;
                morePuzzle.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var moreQuiz = assetItemView.MoreQuiz;
                moreQuiz.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                var moreFeedback = assetItemView.MoreFeedback;
                moreFeedback.fontSize = assetItemView.DefaultDropdownMoreFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    favoriteAdd.fontSize = Mathf.RoundToInt(favoriteAdd.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    favoriteRemove.fontSize = Mathf.RoundToInt(favoriteRemove.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    favoriteTextToSpeech.fontSize = Mathf.RoundToInt(favoriteTextToSpeech.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    moreDownloadTextToSpeech.fontSize = Mathf.RoundToInt(moreDownloadTextToSpeech.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    moreDownload.fontSize = Mathf.RoundToInt(moreDownload.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    moreDownloadDelete.fontSize = Mathf.RoundToInt(moreDownloadDelete.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    moreDescription.fontSize = Mathf.RoundToInt(moreDescription.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    studentNotesText.fontSize = Mathf.RoundToInt(studentNotesText.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    morePuzzle.fontSize = Mathf.RoundToInt(morePuzzle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    moreQuiz.fontSize = Mathf.RoundToInt(moreQuiz.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                    moreFeedback.fontSize = Mathf.RoundToInt(moreFeedback.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
        
        private void UpdateUserContents()
        {
            foreach (var assetItemView in _homeModel.ShownUserContentOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }

        private void UpdateActivityItems()
        {
            foreach (var assetItemView in _homeModel.ShownActivitiesOnHome.Values)
            {
                var activity = assetItemView as IFontSizeView;
                var assetTitle = activity.Title;
                assetTitle.fontSize = activity.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
        
        private void UpdateQuizAssets()
        {
            foreach (var assetItemView in _homeModel.ShownQuizAssetsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
        
        private void UpdatePuzzleAssets()
        {
            foreach (var assetItemView in _homeModel.ShownPuzzleAssetsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
        
        private void UpdateMultipleQuizAssets()
        {
            foreach (var assetItemView in _homeModel.ShownMultipleQuizAssetsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
        
        private void UpdateMultiplePuzzleAssets()
        {
            foreach (var assetItemView in _homeModel.ShownMultiplePuzzleAssetsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
        
        private void UpdateClassificationAssets()
        {
            foreach (var assetItemView in _homeModel.ShownClassificationAssetsOnHome)
            {
                var assetTitle = assetItemView.Title;
                assetTitle.fontSize = assetItemView.DefaultFontSize;
                
                if (_currentFontSize != AccessibilityConstants.FontSizeMedium150)
                {
                    assetTitle.fontSize = Mathf.RoundToInt(assetTitle.fontSize / _accessibilityModel.AssetItemsFontSizeScaler);
                }
            }
        }
    }
}