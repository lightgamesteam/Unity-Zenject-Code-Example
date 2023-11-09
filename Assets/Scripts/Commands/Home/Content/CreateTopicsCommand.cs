using System.Collections.Generic;
using TDL.Constants;
using TDL.Models;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateTopicsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly TopicItemView.Pool _topicPool;

        public void Execute()
        {
            CreateTopics();
        }

        private void CreateTopics()
        {
            foreach (var topicModel in _contentModel.GetTopics())
            {
                var topicView = _topicPool.Spawn(_homeModel.TopicsSubtopicsContent);
                topicView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
                topicView.transform.SetAsLastSibling();

                topicView.ParentId = _contentModel.SelectedSubject.Subject.id;
                topicView.Id = topicModel.Topic.id;
                topicView.Title.text = topicModel.Topic.name;

                SaveTopicOnHomeScreen(topicView);
                
                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(topicView);
                }
                else
                {
                    SetNumberOfAvailableContent((TopicItemViewMobile)topicView, topicModel.Topic.RecursiveTypeCount);
                }
            }
        }
        
        private void SetNumberOfAvailableContent(TopicItemViewMobile topicView, Dictionary<string, int> availableContent)
        {
            topicView.SetVideo2D(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_2D_Video));
            topicView.SetVideo3D(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_3D_Video));
            topicView.SetModel3D(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_3D_Model));
            topicView.SetModules(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_Module));
        }

        private int GetNumberOfAvailableContent(Dictionary<string, int> availableContent, string assetType)
        {
            if (availableContent != null)
            {
                return
                    availableContent.ContainsKey(assetType)
                        ? availableContent[assetType]
                        : 0;
            }

            return 0;
        }

        private void SaveTopicOnHomeScreen(TopicItemView topicView)
        {
            _homeModel.ShownTopicsOnHome.Add(topicView);
        }
        
        private void SetTooltip(TopicItemView topicView)
        {
            var tooltip = topicView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(topicView.Title.text);
        }
    }
}