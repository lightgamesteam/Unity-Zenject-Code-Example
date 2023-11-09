using System.Collections.Generic;
using System.Linq;
using CI.TaskParallel;
using Newtonsoft.Json;
using TDL.Constants;
using Zenject;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using UnityEngine;

namespace TDL.Commands
{
    public class CreateContentModelCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private UserLoginModel _userLoginModel;
        [Inject] private readonly SignalBus _signal;

        // ToDo temporary for Cache prefix changes
        [Inject] private ICacheService _cacheService;

        private List<ClientAssetModel> _favoriteAssets = new List<ClientAssetModel>();
        private List<ClientAssetModel> _recentAssets = new List<ClientAssetModel>();

        private string _assetsCacheFolder;
        private string _filesCacheFolder;
        private bool _isFirstInstall;

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
            var errorMessage = string.Empty;
            ResponseBase errorResponse = null;

            UnityTask.Run(() =>
            {
                var parameter = (CreateContentModelSignal) signal;
                var contentResponse = JsonConvert.DeserializeObject<ContentResponse>(parameter.ContentResponse);
                Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> allAssetDesc = new Dictionary<(int gradeId, int assetId), AssetLocalDesc[]>();
                Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> allAssetStudentDesc = new Dictionary<(int gradeId, int assetId), AssetLocalDesc[]>();

                void AddAssetDescription(Asset asset, int gradeId)
                {
                    if(asset.LocalizedDescription != null && asset.LocalizedDescription.Length > 0)
                        allAssetDesc[(gradeId, asset.Id)] = asset.LocalizedDescription;
                                
                    if(asset.LocalizedStudentDescription != null && asset.LocalizedStudentDescription.Length > 0)
                        allAssetStudentDesc[(gradeId, asset.Id)] = asset.LocalizedStudentDescription;
                }
                
                if (contentResponse.Success)
                {
                    contentResponse.ContentRoot.Grades?.ForEach(grade =>
                    {
                        grade.Subjects?.ForEach(subject =>
                        {
                            subject.Assets?.ForEach(sa =>
                            {
                                AddAssetDescription(sa, grade.id);
                            });
                            
                            subject.Topics?.ForEach(topic =>
                            {
                                topic.Assets?.ForEach(ta =>
                                {
                                    AddAssetDescription(ta, grade.id);
                                });
                                
                                topic.Subtopics?.ForEach(subtopic =>
                                {
                                    subtopic.Assets?.ForEach(a =>
                                    {
                                        AddAssetDescription(a, grade.id);
                                    });
                                });
                            });
                        });
                    });
                    
                    var allActivities = contentResponse.ContentRoot.Activities;

                    var availableSinglePuzzles = GetAvailableActivities(allActivities, ModuleConstants.Module_Puzzle, true);
                    var availableSingleQuizzes = GetAvailableActivities(allActivities, ModuleConstants.Module_Quiz, true);

                    var allClientAssets = new Dictionary<int, ClientAssetModel>();
                    ClientAssetModel CreateAsset(Asset asset, int gradeId, Dictionary<ContentModel.CategoryNames, int> categories)
                    {
                        ClientAssetModel clientAsset;
                            
                        if (allClientAssets.ContainsKey(asset.Id))
                        {
                            clientAsset = allClientAssets[asset.Id];
                            clientAsset.Categories.Add(categories);
                        }
                        else
                        {
                            clientAsset = new ClientAssetModel
                            {
                                Asset = asset,
                                Puzzle = GetAvailableActivityItems(availableSinglePuzzles, asset.Id),
                                Quiz = GetAvailableActivityItems(availableSingleQuizzes, asset.Id),
                                IsAddedToFavorites = asset.IsFavourite,
                                IsRecentlyViewed = asset.IsRecent,
                                HasLessonMode =  asset.LessonMode,
                                LocalizedName = asset.AssetLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name),
                                Categories = new List<Dictionary<ContentModel.CategoryNames, int>>{categories},
                            };
                            
                            allClientAssets.Add(asset.Id, clientAsset);
                        }

                        var favAsset = new ClientAssetModel
                        {
                            Asset = asset,
                            Puzzle = GetAvailableActivityItems(availableSinglePuzzles, asset.Id),
                            Quiz = GetAvailableActivityItems(availableSingleQuizzes, asset.Id),
                            IsAddedToFavorites = asset.IsFavourite,
                            IsRecentlyViewed = asset.IsRecent,
                            HasLessonMode =  asset.LessonMode,
                            LocalizedName = asset.AssetLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name),
                            Categories = new List<Dictionary<ContentModel.CategoryNames, int>>{categories},
                        };
                        
                        favAsset.Asset.GradeId = gradeId;

                        if (favAsset.IsAddedToFavorites)
                        {
                            if (_favoriteAssets.Find(f => f.IsEqualsIds(favAsset)) == null)
                                _favoriteAssets.Add(favAsset);
                        }
                        
                        if (favAsset.IsRecentlyViewed)
                        {
                            if (_recentAssets.Find(f => f.IsEqualsIds(favAsset)) == null)
                                _recentAssets.Add(favAsset);
                        }

                        return clientAsset;
                    }

                    var clientGrades = new List<ClientGradeModel>();

                    if (contentResponse.ContentRoot != null)
                    {
                        foreach (var grade in contentResponse.ContentRoot.Grades)
                        {
                            var clientGrade = new ClientGradeModel
                            {
                                Id = grade.id,
                                Grade = grade
                            };

                            if (clientGrade.Grade.Subjects != null)
                            {
                                foreach (var subject in clientGrade.Grade.Subjects)
                                {
                                    var clientSubject = new ClientSubjectModel
                                    {
                                        Id = subject.id,
                                        ParentGrade = clientGrade,
                                        Subject = subject,
                                    };

                                    if (subject.Assets != null && subject.Assets.Count > 0)
                                    {
                                        clientSubject.AssetsInCategory = subject.Assets
                                            .Select(asset => CreateAsset(asset, grade.id,
                                                new Dictionary<ContentModel.CategoryNames, int>
                                                {
                                                    {ContentModel.CategoryNames.GradeId, grade.id},
                                                    {ContentModel.CategoryNames.SubjectId, subject.id},
                                                    {ContentModel.CategoryNames.TopicId, 0},
                                                    {ContentModel.CategoryNames.SubtopicId, 0}
                                                })).ToList();
                                    }

                                    if (subject.Topics != null)
                                    {
                                        foreach (var topic in subject.Topics)
                                        {
                                            var clientTopic = new ClientTopicModel
                                            {
                                                Id = topic.id,
                                                ParentSubject = clientSubject,
                                                Topic = topic
                                            };

                                            if (topic.Assets != null && topic.Assets.Count > 0)
                                            {
                                                clientTopic.AssetsInCategory = topic.Assets.Select(asset =>
                                                    CreateAsset(asset, grade.id,
                                                        new Dictionary<ContentModel.CategoryNames, int>
                                                        {
                                                            {ContentModel.CategoryNames.GradeId, grade.id},
                                                            {ContentModel.CategoryNames.SubjectId, clientSubject.Id},
                                                            {ContentModel.CategoryNames.TopicId, topic.id},
                                                            {ContentModel.CategoryNames.SubtopicId, 0}
                                                        })).ToList();
                                            }

                                            if (topic.Subtopics != null)
                                            {
                                                foreach (var subtopic in topic.Subtopics)
                                                {
                                                    var clientSubtopic = new ClientSubtopicModel
                                                    {
                                                        Id = subtopic.id,
                                                        ParentTopic = clientTopic,
                                                        Subtopic = subtopic
                                                    };

                                                    if (subtopic.Assets != null && subtopic.Assets.Count > 0)
                                                    {
                                                        clientSubtopic.AssetsInCategory = subtopic.Assets
                                                            .Select(asset => CreateAsset(asset, grade.id,
                                                                new Dictionary<ContentModel.CategoryNames, int>
                                                                {
                                                                    {ContentModel.CategoryNames.GradeId, grade.id},
                                                                    {ContentModel.CategoryNames.SubjectId, clientSubject.Id},
                                                                    {ContentModel.CategoryNames.TopicId, clientTopic.Id},
                                                                    {ContentModel.CategoryNames.SubtopicId, subtopic.id}
                                                                })).ToList();
                                                    }

                                                    clientSubtopic.AllAssets = clientSubtopic.AssetsInCategory;
                                                    clientTopic.ClientSubtopics.Add(clientSubtopic);
                                                    clientTopic.AssetsInChildren.AddRange(clientSubtopic.AssetsInCategory);
                                                    clientTopic.AllAssets.AddRange(clientSubtopic.AssetsInCategory);
                                                }
                                            }

                                            clientSubject.ClientTopics.Add(clientTopic);
                                            clientTopic.AllAssets.AddRange(clientTopic.AssetsInCategory);
                                            clientSubject.AssetsInChildren.AddRange(clientTopic.AssetsInCategory);
                                            clientSubject.AssetsInChildren.AddRange(clientTopic.AssetsInChildren);
                                            clientSubject.AllAssets.AddRange(clientTopic.AssetsInCategory);
                                            clientSubject.AllAssets.AddRange(clientTopic.AssetsInChildren);
                                        }
                                    }

                                    clientSubject.AllAssets.AddRange(clientSubject.AssetsInCategory);

                                    clientGrade.ClientSubjects.Add(clientSubject);
                                    clientGrade.AssetsInChildren.AddRange(clientSubject.AllAssets);
                                    clientGrade.AllAssets.AddRange(clientSubject.AllAssets);
                                }
                            }

                            clientGrade.AllAssets.AddRange(clientGrade.AssetsInCategory);
                            clientGrades.Add(clientGrade);
                        }
                    }

                    var availableMultiplePuzzles = GetAvailableActivities(allActivities, ModuleConstants.Module_Puzzle, false, allClientAssets);
                    var availableMultiQuizzes = GetAvailableActivities(allActivities, ModuleConstants.Module_Quiz, false, allClientAssets);
                    var availableClassifications = GetAvailableActivities(allActivities, ModuleConstants.Module_Classification, false, allClientAssets);

                    SetDirectPathToActivity(availableMultiQuizzes);
                    SetDirectPathToActivity(availableMultiplePuzzles);
                    SetDirectPathToActivity(availableClassifications);

                    var contentModelStruct = new ContentModel.ContentModelStruct
                    {
                        RecentAssets = _recentAssets,
                        FavoriteAssets = _favoriteAssets,
                        MultiplePuzzles = availableMultiplePuzzles,
                        MultipleQuizzes = availableMultiQuizzes,
                        Classifications = availableClassifications,
                        Grades = clientGrades,
                        AllAssets = allClientAssets,
                        AllAssetLocalDesc = allAssetDesc,
                        AllAssetLocalStudentDesc = allAssetStudentDesc
                    };


                    return contentModelStruct;
                }

                errorResponse = contentResponse;
                errorMessage = contentResponse.ErrorMessage;
                
                return null;

            }).ContinueOnUIThread(task =>
            {
                if (task.Result != null)
                {
                    _contentModel.UpdateMainContent(task.Result);
                }
                else
                {
                    _userLoginModel.ErrorMessages = errorResponse.LocalizedError;
                    
                    _signal.Fire(new PopupOverlaySignal(false));
                    _signal.Fire(new SendCrashAnalyticsCommandSignal("CreateContentModelCommand server response | " + errorMessage));
                }
            });
        }
        
        private void RunInBackgroundWebGL(ISignal signal)
        {
            var errorMessage = string.Empty;
            ResponseBase errorResponse = null;

            //UnityTask.Run(() =>
            {
                var parameter = (CreateContentModelSignal) signal;
                var contentResponse = JsonConvert.DeserializeObject<ContentResponse>(parameter.ContentResponse);
                Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> allAssetDesc = new Dictionary<(int gradeId, int assetId), AssetLocalDesc[]>();
                Dictionary<(int gradeId, int assetId), AssetLocalDesc[]> allAssetStudentDesc = new Dictionary<(int gradeId, int assetId), AssetLocalDesc[]>();

                void AddAssetDescription(Asset asset, int gradeId)
                {
                    if(asset.LocalizedDescription != null && asset.LocalizedDescription.Length > 0)
                        allAssetDesc[(gradeId, asset.Id)] = asset.LocalizedDescription;
                                
                    if(asset.LocalizedStudentDescription != null && asset.LocalizedStudentDescription.Length > 0)
                        allAssetStudentDesc[(gradeId, asset.Id)] = asset.LocalizedStudentDescription;
                }
                
                if (contentResponse.Success)
                {
                    contentResponse.ContentRoot.Grades?.ForEach(grade =>
                    {
                        grade.Subjects?.ForEach(subject =>
                        {
                            subject.Assets?.ForEach(sa =>
                            {
                                AddAssetDescription(sa, grade.id);
                            });
                            
                            subject.Topics?.ForEach(topic =>
                            {
                                topic.Assets?.ForEach(ta =>
                                {
                                    AddAssetDescription(ta, grade.id);
                                });
                                
                                topic.Subtopics?.ForEach(subtopic =>
                                {
                                    subtopic.Assets?.ForEach(a =>
                                    {
                                        AddAssetDescription(a, grade.id);
                                    });
                                });
                            });
                        });
                    });
                    
                    var allActivities = contentResponse.ContentRoot.Activities;

                    var availableSinglePuzzles = GetAvailableActivities(allActivities, ModuleConstants.Module_Puzzle, true);
                    var availableSingleQuizzes = GetAvailableActivities(allActivities, ModuleConstants.Module_Quiz, true);

                    var allClientAssets = new Dictionary<int, ClientAssetModel>();
                    ClientAssetModel CreateAsset(Asset asset, int gradeId, Dictionary<ContentModel.CategoryNames, int> categories)
                    {
                        ClientAssetModel clientAsset;
                            
                        if (allClientAssets.ContainsKey(asset.Id))
                        {
                            clientAsset = allClientAssets[asset.Id];
                            clientAsset.Categories.Add(categories);
                        }
                        else
                        {
                            clientAsset = new ClientAssetModel
                            {
                                Asset = asset,
                                Puzzle = GetAvailableActivityItems(availableSinglePuzzles, asset.Id),
                                Quiz = GetAvailableActivityItems(availableSingleQuizzes, asset.Id),
                                IsAddedToFavorites = asset.IsFavourite,
                                IsRecentlyViewed = asset.IsRecent,
                                HasLessonMode =  asset.LessonMode,
                                LocalizedName = asset.AssetLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name),
                                Categories = new List<Dictionary<ContentModel.CategoryNames, int>>{categories},
                            };
                            
                            allClientAssets.Add(asset.Id, clientAsset);
                        }

                        var favAsset = new ClientAssetModel
                        {
                            Asset = asset,
                            Puzzle = GetAvailableActivityItems(availableSinglePuzzles, asset.Id),
                            Quiz = GetAvailableActivityItems(availableSingleQuizzes, asset.Id),
                            IsAddedToFavorites = asset.IsFavourite,
                            IsRecentlyViewed = asset.IsRecent,
                            HasLessonMode =  asset.LessonMode,
                            LocalizedName = asset.AssetLocal.ToDictionary(assetLocale => assetLocale.Culture, assetLocale => assetLocale.Name),
                            Categories = new List<Dictionary<ContentModel.CategoryNames, int>>{categories},
                        };
                        
                        favAsset.Asset.GradeId = gradeId;

                        if (favAsset.IsAddedToFavorites)
                        {
                            if (_favoriteAssets.Find(f => f.IsEqualsIds(favAsset)) == null)
                                _favoriteAssets.Add(favAsset);
                        }
                        
                        if (favAsset.IsRecentlyViewed)
                        {
                            if (_recentAssets.Find(f => f.IsEqualsIds(favAsset)) == null)
                                _recentAssets.Add(favAsset);
                        }

                        return clientAsset;
                    }

                    var clientGrades = new List<ClientGradeModel>();

                    if (contentResponse.ContentRoot != null)
                    {
                        foreach (var grade in contentResponse.ContentRoot.Grades)
                        {
                            var clientGrade = new ClientGradeModel
                            {
                                Id = grade.id,
                                Grade = grade
                            };

                            if (clientGrade.Grade.Subjects != null)
                            {
                                foreach (var subject in clientGrade.Grade.Subjects)
                                {
                                    var clientSubject = new ClientSubjectModel
                                    {
                                        Id = subject.id,
                                        ParentGrade = clientGrade,
                                        Subject = subject,
                                    };

                                    if (subject.Assets != null && subject.Assets.Count > 0)
                                    {
                                        clientSubject.AssetsInCategory = subject.Assets
                                            .Select(asset => CreateAsset(asset, grade.id,
                                                new Dictionary<ContentModel.CategoryNames, int>
                                                {
                                                    {ContentModel.CategoryNames.GradeId, grade.id},
                                                    {ContentModel.CategoryNames.SubjectId, subject.id},
                                                    {ContentModel.CategoryNames.TopicId, 0},
                                                    {ContentModel.CategoryNames.SubtopicId, 0}
                                                })).ToList();
                                    }

                                    if (subject.Topics != null)
                                    {
                                        foreach (var topic in subject.Topics)
                                        {
                                            var clientTopic = new ClientTopicModel
                                            {
                                                Id = topic.id,
                                                ParentSubject = clientSubject,
                                                Topic = topic
                                            };

                                            if (topic.Assets != null && topic.Assets.Count > 0)
                                            {
                                                clientTopic.AssetsInCategory = topic.Assets.Select(asset =>
                                                    CreateAsset(asset, grade.id,
                                                        new Dictionary<ContentModel.CategoryNames, int>
                                                        {
                                                            {ContentModel.CategoryNames.GradeId, grade.id},
                                                            {ContentModel.CategoryNames.SubjectId, clientSubject.Id},
                                                            {ContentModel.CategoryNames.TopicId, topic.id},
                                                            {ContentModel.CategoryNames.SubtopicId, 0}
                                                        })).ToList();
                                            }

                                            if (topic.Subtopics != null)
                                            {
                                                foreach (var subtopic in topic.Subtopics)
                                                {
                                                    var clientSubtopic = new ClientSubtopicModel
                                                    {
                                                        Id = subtopic.id,
                                                        ParentTopic = clientTopic,
                                                        Subtopic = subtopic
                                                    };

                                                    if (subtopic.Assets != null && subtopic.Assets.Count > 0)
                                                    {
                                                        clientSubtopic.AssetsInCategory = subtopic.Assets
                                                            .Select(asset => CreateAsset(asset, grade.id,
                                                                new Dictionary<ContentModel.CategoryNames, int>
                                                                {
                                                                    {ContentModel.CategoryNames.GradeId, grade.id},
                                                                    {ContentModel.CategoryNames.SubjectId, clientSubject.Id},
                                                                    {ContentModel.CategoryNames.TopicId, clientTopic.Id},
                                                                    {ContentModel.CategoryNames.SubtopicId, subtopic.id}
                                                                })).ToList();
                                                    }

                                                    clientSubtopic.AllAssets = clientSubtopic.AssetsInCategory;
                                                    clientTopic.ClientSubtopics.Add(clientSubtopic);
                                                    clientTopic.AssetsInChildren.AddRange(clientSubtopic.AssetsInCategory);
                                                    clientTopic.AllAssets.AddRange(clientSubtopic.AssetsInCategory);
                                                }
                                            }

                                            clientSubject.ClientTopics.Add(clientTopic);
                                            clientTopic.AllAssets.AddRange(clientTopic.AssetsInCategory);
                                            clientSubject.AssetsInChildren.AddRange(clientTopic.AssetsInCategory);
                                            clientSubject.AssetsInChildren.AddRange(clientTopic.AssetsInChildren);
                                            clientSubject.AllAssets.AddRange(clientTopic.AssetsInCategory);
                                            clientSubject.AllAssets.AddRange(clientTopic.AssetsInChildren);
                                        }
                                    }

                                    clientSubject.AllAssets.AddRange(clientSubject.AssetsInCategory);

                                    clientGrade.ClientSubjects.Add(clientSubject);
                                    clientGrade.AssetsInChildren.AddRange(clientSubject.AllAssets);
                                    clientGrade.AllAssets.AddRange(clientSubject.AllAssets);
                                }
                            }

                            clientGrade.AllAssets.AddRange(clientGrade.AssetsInCategory);
                            clientGrades.Add(clientGrade);
                        }
                    }

                    var availableMultiplePuzzles = GetAvailableActivities(allActivities, ModuleConstants.Module_Puzzle, false, allClientAssets);
                    var availableMultiQuizzes = GetAvailableActivities(allActivities, ModuleConstants.Module_Quiz, false, allClientAssets);
                    var availableClassifications = GetAvailableActivities(allActivities, ModuleConstants.Module_Classification, false, allClientAssets);

                    SetDirectPathToActivity(availableMultiQuizzes);
                    SetDirectPathToActivity(availableMultiplePuzzles);
                    SetDirectPathToActivity(availableClassifications);

                    var contentModelStruct = new ContentModel.ContentModelStruct
                    {
                        RecentAssets = _recentAssets,
                        FavoriteAssets = _favoriteAssets,
                        MultiplePuzzles = availableMultiplePuzzles,
                        MultipleQuizzes = availableMultiQuizzes,
                        Classifications = availableClassifications,
                        Grades = clientGrades,
                        AllAssets = allClientAssets,
                        AllAssetLocalDesc = allAssetDesc,
                        AllAssetLocalStudentDesc = allAssetStudentDesc
                    };

                    AfterTask(contentModelStruct, errorResponse,errorMessage );
                    return;
                }

                errorResponse = contentResponse;
                errorMessage = contentResponse.ErrorMessage;

                AfterTask(null, errorResponse,errorMessage );
            }
        }

        private void AfterTask(ContentModel.ContentModelStruct task, ResponseBase errorResponse, string errorMessage)
        {
            if (task != null)
            {
                _contentModel.UpdateMainContent(task);
            }
            else
            {
                _userLoginModel.ErrorMessages = errorResponse.LocalizedError;
                    
                _signal.Fire(new PopupOverlaySignal(false));
                _signal.Fire(new SendCrashAnalyticsCommandSignal("CreateContentModelCommand server response | " + errorMessage));
            }
        }

        private List<ActivityItem> GetAvailableActivityItems(List<ClientActivityModel> activityItems, int assetId)
        {
            var foundedActivityItems = new List<ActivityItem>();
            foreach (var clientActivityItem in activityItems)
            {
                var foundItem =
                    clientActivityItem.ActivityItem.assetContent?.FirstOrDefault(item => item.assetId == assetId);
                if (foundItem != null)
                {
                    foundedActivityItems.Add(clientActivityItem.ActivityItem);
                }
            }

            return foundedActivityItems;
        }

        private List<ClientActivityModel> GetAvailableActivities(List<ActivityModel> allActivities, string activityName, bool isSingleActivity, Dictionary<int, ClientAssetModel> allAssets = null)
        {
            var activities = allActivities.FirstOrDefault(activity => activity.ActivityName.Equals(activityName));
            var availableActivities = activities?.ActivityItem
                ?.Where(activityItem => activityItem.isModelSpecific == isSingleActivity);

            if (availableActivities != null)
            {
                var clientAvailableActivities = new List<ClientActivityModel>();
                foreach (var activity in availableActivities)
                {
                    var clientActivity = new ClientActivityModel
                    {
                        ActivityItem = activity
                    };

                    clientAvailableActivities.Add(clientActivity);
                }

                return clientAvailableActivities;
            }

            return null;
        }

        private void SetDirectPathToActivity(List<ClientActivityModel> activityModels)
        {
            foreach (var activityModel in activityModels)
            {
                foreach (var activityDetail in activityModel.ActivityItem.ActivityDetails)
                {
                    var categoryPaths = new Dictionary<ContentModel.CategoryNames, int>
                    {
                        {ContentModel.CategoryNames.GradeId, activityDetail.GradeId},
                        {ContentModel.CategoryNames.SubjectId, activityDetail.SubjectId},
                        {ContentModel.CategoryNames.TopicId, activityDetail.TopicId},
                        {ContentModel.CategoryNames.SubtopicId, activityDetail.SubtopicId}
                    };

                    activityModel.DirectPathToActivity.Add(categoryPaths);
                    _contentModel.ActivityDetails.Add(activityDetail);
                }
            }
        }
    }
}