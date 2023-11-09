using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SaveContentPanelsCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveContentPanelsCommandSignal) signal;
            _homeModel.LeftMenuContent = parameter.LeftMenuContent;
            _homeModel.TopicsSubtopicsContent = parameter.TopicsSubtopicsContent;
            _homeModel.AssetsContent = parameter.AssetsContent;
            _homeModel.MainContent = _homeModel.AssetsContent.parent.GetComponent<RectTransform>();
        }
    }
}