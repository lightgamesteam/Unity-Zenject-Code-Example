using TDL.Models;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class ClearAllContentCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;
        [Inject] private readonly TopicItemView.Pool _topicPool;
        [Inject] private readonly SubtopicItemView.Pool _subtopicPool;
        [Inject] private readonly AssetItemView.Pool _assetPool;
        [Inject] private readonly QuizAssetView.Pool _assetQuizPool;
        [Inject] private readonly PuzzleAssetItemView.Pool _assetPuzzlePool;
        [Inject] private readonly MultipleQuizAssetItemView.Pool _multipleQuizPool;
        [Inject] private readonly MultiplePuzzleAssetItemView.Pool _multiplePuzzlePool;
        [Inject] private readonly ClassificationAssetItemView.Pool _classificationPool;
        
        // activity items
        [Inject] private readonly ActivityItemView.Pool _activityItemPool;
        [Inject] private readonly ActivityQuizView.Pool _activityQuizPool;
        [Inject] private readonly ActivityPuzzleView.Pool _activityPuzzlePool;
        [Inject] private readonly ActivityMultipleQuizView.Pool _activityMultipleQuizPool;
        [Inject] private readonly ActivityMultiplePuzzlesView.Pool _activityMultiplePuzzlePool;
        [Inject] private readonly ActivityClassificationView.Pool _activityClassificationPool;
        
        [InjectOptional] private readonly  GradeItemView.Pool _gradePool;
        [InjectOptional] private readonly  SubjectItemView.Pool _subjectPool;
        [InjectOptional] private readonly UserContentItemView.Pool _userContentItemViewPool;

        public void Execute()
        {
            if (DeviceInfo.IsMobile())
            {
                ClearGrades();
                ClearSubjects();
            }
            
            ClearTopics();
            ClearSubtopics();
            ClearAssets();
            ClearActivityItems();
            ClearQuizAssets();
            ClearPuzzleAssets();
            ClearMultipleQuizAssets();
            ClearMultiplePuzzleAssets();
            ClearClassificationAssets();
            ClearNoSearchResults();
            ClearCachedTextures();
            ClearUserContents();
        }

        private void ClearGrades()
        {
            var activeObjectsInPool = _gradePool.NumActive;

            for (var index = 0; index < activeObjectsInPool; index++)
            {
                var grade = _homeModel.ShownGradeOnHome[index];
                _gradePool.Despawn(grade);
            }

            _homeModel.ShownGradeOnHome.Clear();
        }

        private void ClearSubjects()
        {
            var activeObjectsInPool = _subjectPool.NumActive;

            for (var index = 0; index < activeObjectsInPool; index++)
            {
                var subject = _homeModel.ShownSubjectOnHome[index];
                _subjectPool.Despawn(subject);
            }

            _homeModel.ShownSubjectOnHome.Clear();
        }
        
        private void ClearTopics()
        {
            var activeObjectsInPool = _topicPool.NumActive;

            for (var index = 0; index < activeObjectsInPool; index++)
            {
                var topic = _homeModel.ShownTopicsOnHome[index];
                _topicPool.Despawn(topic);
            }

            _homeModel.ShownTopicsOnHome.Clear();
        }

        private void ClearSubtopics()
        {
            var activeObjectsInPool = _subtopicPool.NumActive;

            for (var index = 0; index < activeObjectsInPool; index++)
            {
                var subtopic = _homeModel.ShownSubtopicsOnHome[index];
                _subtopicPool.Despawn(subtopic);
            }

            _homeModel.ShownSubtopicsOnHome.Clear();
        }

        private void ClearAssets()
        {
            var activeObjectsInPool = _assetPool.NumActive;

            for (var index = 0; index < activeObjectsInPool; index++)
            {
                var asset = _homeModel.ShownAssetsOnHome[index];
                _assetPool.Despawn(asset);
            }

            _homeModel.ShownAssetsOnHome.Clear();
        }
        
        private void ClearUserContents()
        {
            var activeObjectsInPool = _userContentItemViewPool.NumActive;

            for (var index = 0; index < activeObjectsInPool; index++)
            {
                var asset = _homeModel.ShownUserContentOnHome[index];
                _userContentItemViewPool.Despawn(asset);
            }

            _homeModel.ShownUserContentOnHome.Clear();
        }

        private void ClearActivityItems()
        {
            var activityItemPool = _activityItemPool.NumActive;
            for (var index = 0; index < activityItemPool; index++)
            {
                var asset = _homeModel.ShownActivitiesOnHome[typeof(ActivityItemView).Name];
                _activityItemPool.Despawn((ActivityItemView) asset);
            }

            var activityQuiz = _activityQuizPool.NumActive;
            for (var index = 0; index < activityQuiz; index++)
            {
                var asset = _homeModel.ShownActivitiesOnHome[typeof(ActivityQuizView).Name];
                _activityQuizPool.Despawn((ActivityQuizView) asset);
            }
            
            var activityPuzzle = _activityPuzzlePool.NumActive;
            for (var index = 0; index < activityPuzzle; index++)
            {
                var asset = _homeModel.ShownActivitiesOnHome[typeof(ActivityPuzzleView).Name];
                _activityPuzzlePool.Despawn((ActivityPuzzleView) asset);
            }
            
            var activityMultipleQuiz = _activityMultipleQuizPool.NumActive;
            for (var index = 0; index < activityMultipleQuiz; index++)
            {
                var asset = _homeModel.ShownActivitiesOnHome[typeof(ActivityMultipleQuizView).Name];
                _activityMultipleQuizPool.Despawn((ActivityMultipleQuizView) asset);
            }
            
            var activityMultiplePuzzle = _activityMultiplePuzzlePool.NumActive;
            for (var index = 0; index < activityMultiplePuzzle; index++)
            {
                var asset = _homeModel.ShownActivitiesOnHome[typeof(ActivityMultiplePuzzlesView).Name];
                _activityMultiplePuzzlePool.Despawn((ActivityMultiplePuzzlesView) asset);
            }
            
            var activityClassification = _activityClassificationPool.NumActive;
            for (var index = 0; index < activityClassification; index++)
            {
                var asset = _homeModel.ShownActivitiesOnHome[typeof(ActivityClassificationView).Name];
                _activityClassificationPool.Despawn((ActivityClassificationView) asset);
            }
            
            _homeModel.ShownActivitiesOnHome.Clear();
        }
        
        private void ClearQuizAssets()
        {
            var assetQuizPool = _assetQuizPool.NumActive;

            for (var index = 0; index < assetQuizPool; index++)
            {
                var item = _homeModel.ShownQuizAssetsOnHome[index];
                _assetQuizPool.Despawn(item);
            }

            _homeModel.ShownQuizAssetsOnHome.Clear();
        }
        
        private void ClearPuzzleAssets()
        {
            var assetPuzzlePool = _assetPuzzlePool.NumActive;

            for (var index = 0; index < assetPuzzlePool; index++)
            {
                var item = _homeModel.ShownPuzzleAssetsOnHome[index];
                _assetPuzzlePool.Despawn(item);
            }

            _homeModel.ShownPuzzleAssetsOnHome.Clear();
        }
        
        private void ClearMultipleQuizAssets()
        {
            var assetQuizPool = _multipleQuizPool.NumActive;

            for (var index = 0; index < assetQuizPool; index++)
            {
                var item = _homeModel.ShownMultipleQuizAssetsOnHome[index];
                _multipleQuizPool.Despawn(item);
            }

            _homeModel.ShownMultipleQuizAssetsOnHome.Clear();
        }
        
        private void ClearMultiplePuzzleAssets()
        {
            var assetPuzzlePool = _multiplePuzzlePool.NumActive;

            for (var index = 0; index < assetPuzzlePool; index++)
            {
                var item = _homeModel.ShownMultiplePuzzleAssetsOnHome[index];
                _multiplePuzzlePool.Despawn(item);
            }

            _homeModel.ShownMultiplePuzzleAssetsOnHome.Clear();
        }
        
        private void ClearClassificationAssets()
        {
            var classificationPool = _classificationPool.NumActive;

            for (var index = 0; index < classificationPool; index++)
            {
                var item = _homeModel.ShownClassificationAssetsOnHome[index];
                _classificationPool.Despawn(item);
            }

            _homeModel.ShownClassificationAssetsOnHome.Clear();
        }
        
        private void ClearNoSearchResults()
        {
            _homeModel.ShowNoSearchResults = false;
        }
        
        private void ClearCachedTextures()
        {
            _contentModel.ClearCachedTextures();
        }
    }
}