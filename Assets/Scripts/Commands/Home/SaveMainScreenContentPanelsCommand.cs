using TDL.Models;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SaveMainScreenContentPanelsCommand : ICommandWithParameters
    {
        [Inject] private MainScreenModel _mainModel;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveMainScreenContentPanelsCommandSignal) signal;
            _mainModel.AssetsContent = parameter.AssetsContent;
            _mainModel.MainContent = _mainModel.AssetsContent.parent.GetComponent<RectTransform>();

            _mainModel.GridAdapter = parameter.GridAdapter;
        }
    }
}