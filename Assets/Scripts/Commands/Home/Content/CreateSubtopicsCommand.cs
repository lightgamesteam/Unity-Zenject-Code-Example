using System.Collections.Generic;
using TDL.Constants;
using TDL.Models;
using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class CreateSubtopicsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly SubtopicItemView.Pool _subtopicPool;
        
        public void Execute()
        {
            CreateSubtopics();
        }
        
        private void CreateSubtopics()
        {
            foreach (var subtopicModel in _contentModel.GetSubtopics())
            {
                var subtopicView = _subtopicPool.Spawn(_homeModel.TopicsSubtopicsContent);
                subtopicView.transform.SetParent(_homeModel.TopicsSubtopicsContent, false);
                subtopicView.transform.SetAsLastSibling();
                
                subtopicView.ParentId = _contentModel.SelectedTopic.Topic.id;
                subtopicView.Id = subtopicModel.Subtopic.id;
                subtopicView.Title.text = subtopicModel.Subtopic.name;
                SetNumberOfAvailableContent(subtopicView, subtopicModel.Subtopic.RecursiveTypeCount);
                SaveSubtopicOnHomeScreen(subtopicView);

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(subtopicView);
                }
            }
        }

        private void SetNumberOfAvailableContent(SubtopicItemView subtopicView, Dictionary<string, int> availableContent)
        {
            subtopicView.SetVideo2D(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_2D_Video));
            subtopicView.SetVideo3D(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_3D_Video));
            subtopicView.SetModel3D(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_3D_Model));
            subtopicView.SetModel360(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_360_Model));
            subtopicView.SetModelRigged(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_Rigged_Model));
            subtopicView.SetModules(GetNumberOfAvailableContent(availableContent, AssetTypeConstants.Type_Module));
        }

        private int GetNumberOfAvailableContent(Dictionary<string, int> availableContent, string assetType)
        {
            return 
                availableContent.ContainsKey(assetType)
                    ? availableContent[assetType]
                    : 0;
        }
        
        private void SaveSubtopicOnHomeScreen(SubtopicItemView subtopicView)
        {
            _homeModel.ShownSubtopicsOnHome.Add(subtopicView);
        }
        
        private void SetTooltip(SubtopicItemView subtopicView)
        {
            var tooltip = subtopicView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(subtopicView.Title.text);
        }
    }
}