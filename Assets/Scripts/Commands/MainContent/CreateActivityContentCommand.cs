using System.Collections.Generic;
using CI.TaskParallel;
using Newtonsoft.Json;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class CreateActivityContentCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;

        private bool _isQuizReady;
        private bool _isPuzzleReady;
        private bool _isClassificationReady;
        
        private HashSet<int> _commonCategoryID = new HashSet<int>();
        private HashSet<int> _commonParentCategoryID = new HashSet<int>();

        public void Execute(ISignal signal)
        {
#if UNITY_WEBGL
            RunInBackgroundWebGL(signal);
#else
            RunInBackground(signal);
#endif
        }

        private void RunInBackground(ISignal signal)
        {
            UnityTask.Run(() =>
            {
                var parameter = (CreateActivityContentCommandSignal) signal;

                var activityResponse = JsonConvert.DeserializeObject<ActivityItem>(parameter.ContentResponse);

                var availableMultiQuiz = new List<ClientActivityModel>();
                var availableMultiPuzzle = new List<ClientActivityModel>();
                var availableClassifications = new List<ClientActivityModel>();

                switch (parameter.ActivityName)
                {
                    case ModuleConstants.Module_Quiz:

//                        foreach (var quizActivity in activityResponse)
//                        {
                            if (IsSingleActivity(activityResponse))
                            {
                                SetSingleActivity(ModuleConstants.Module_Quiz, activityResponse);
                            }
                            else
                            {
                                SetMultiActivities(ModuleConstants.Module_Quiz, activityResponse, availableMultiQuiz);
                            }
//                        }

                        _isQuizReady = true;
                        break;

                    case ModuleConstants.Module_Puzzle:

//                        foreach (var puzzleActivity in activityResponse)
//                        {
                            if (IsSingleActivity(activityResponse))
                            {
//                                Debug.Log("Single ItemID: " + puzzleActivity.itemId);
                                SetSingleActivity(ModuleConstants.Module_Puzzle, activityResponse);
                            }
                            else
                            {
//                                Debug.Log("Multi ItemID: " + puzzleActivity.itemId);

                                SetMultiActivities(ModuleConstants.Module_Puzzle, activityResponse, availableMultiPuzzle);
//                            }
                        }
                        
                        _isPuzzleReady = true;
                        break;

                    case ModuleConstants.Module_Classification:
                        
//                        foreach (var classificationActivity in activityResponse)
//                        {
//                            Debug.Log("ID: " + classificationActivity.itemId);
                            
//                            if (classificationActivity.itemId == 220)
//                            {
//                                string str = "go";
//                            }
                            
                            SetMultiActivities(ModuleConstants.Module_Classification, activityResponse, availableClassifications);
//                        }

                        _isClassificationReady = true;
                        break;
                    
                }
            }).ContinueOnUIThread(task =>
            {
                if (_isQuizReady && _isPuzzleReady && _isClassificationReady)
                {
                    _contentModel.NotifyUpdated();
                }
            });
        }
        
        private void RunInBackgroundWebGL(ISignal signal)
        {

                var parameter = (CreateActivityContentCommandSignal) signal;

                var activityResponse = JsonConvert.DeserializeObject<ActivityItem>(parameter.ContentResponse);

                var availableMultiQuiz = new List<ClientActivityModel>();
                var availableMultiPuzzle = new List<ClientActivityModel>();
                var availableClassifications = new List<ClientActivityModel>();

                switch (parameter.ActivityName)
                {
                    case ModuleConstants.Module_Quiz:

//                        foreach (var quizActivity in activityResponse)
//                        {
                            if (IsSingleActivity(activityResponse))
                            {
                                SetSingleActivity(ModuleConstants.Module_Quiz, activityResponse);
                            }
                            else
                            {
                                SetMultiActivities(ModuleConstants.Module_Quiz, activityResponse, availableMultiQuiz);
                            }
//                        }

                        _isQuizReady = true;
                        break;

                    case ModuleConstants.Module_Puzzle:

//                        foreach (var puzzleActivity in activityResponse)
//                        {
                            if (IsSingleActivity(activityResponse))
                            {
//                                Debug.Log("Single ItemID: " + puzzleActivity.itemId);
                                SetSingleActivity(ModuleConstants.Module_Puzzle, activityResponse);
                            }
                            else
                            {
//                                Debug.Log("Multi ItemID: " + puzzleActivity.itemId);

                                SetMultiActivities(ModuleConstants.Module_Puzzle, activityResponse, availableMultiPuzzle);
//                            }
                        }
                        
                        _isPuzzleReady = true;
                        break;

                    case ModuleConstants.Module_Classification:
                        
//                        foreach (var classificationActivity in activityResponse)
//                        {
//                            Debug.Log("ID: " + classificationActivity.itemId);
                            
//                            if (classificationActivity.itemId == 220)
//                            {
//                                string str = "go";
//                            }
                            
                            SetMultiActivities(ModuleConstants.Module_Classification, activityResponse, availableClassifications);
//                        }

                        _isClassificationReady = true;
                        break;
                    
                }

                if (_isQuizReady && _isPuzzleReady && _isClassificationReady)
                {
                    _contentModel.NotifyUpdated();
                }

        }

        private bool IsSingleActivity(ActivityItem activityItem)
        {
            return activityItem.isModelSpecific;
        }

        private void SetSingleActivity(string activityType, ActivityItem activityItem)
        {
            if (activityItem.assetContent != null && activityItem.assetContent.Count > 0)
            {
                var puzzleAssetContent = activityItem.assetContent[0];
                var asset = _contentModel.GetAssetById(puzzleAssetContent.assetId);
                if (asset != null)
                {
                    switch (activityType)
                    {
                        case ModuleConstants.Module_Quiz:
                            asset.Quiz = new List<ActivityItem>{activityItem};
                            break;
                        
                        case ModuleConstants.Module_Puzzle:
                            asset.Puzzle = new List<ActivityItem>{activityItem};;
                            break;
                    }
                }
            }
        }

        private void SetMultiActivities(string activityType, ActivityItem activityItem, List<ClientActivityModel> availableActivities)
        {
//            var clientActivityItem = new ClientActivityModel
//            {
//                Id = activityItem.itemId,
//                ActivityItem = activityItem
//            };
//
//            foreach (var activityContent in clientActivityItem.ActivityItem.assetContent)
//            {
//                var foundedAsset = _contentModel.GetAssetById(activityContent.assetId);
//
//                if (foundedAsset != null)
//                {
//                    _commonCategoryID = foundedAsset.CategoryIds;
//                    _commonParentCategoryID = foundedAsset.ParentCategoryIds;   
//                }
//                else
//                {
//                    Debug.Log("No asset: " + activityContent.assetId + " for " + activityItem.itemName + " classification");
//                }
//            }
//
//            clientActivityItem.CategoryIds = _commonCategoryID;
//            clientActivityItem.ParentCategoryIds = _commonParentCategoryID;
//                                
//            availableActivities.Add(clientActivityItem);
//            
//            switch (activityType)
//            {
//                case ModuleConstants.Module_Quiz:
//                    _contentModel.MultipleQuiz = availableActivities;
//                    break;
//                        
//                case ModuleConstants.Module_Puzzle:
//                    _contentModel.MultiplePuzzle = availableActivities;
//                    break;
//                
//                case ModuleConstants.Module_Classification:
//                    _contentModel.Classifications = availableActivities;
//                    break;
//            }
        }
    }
}