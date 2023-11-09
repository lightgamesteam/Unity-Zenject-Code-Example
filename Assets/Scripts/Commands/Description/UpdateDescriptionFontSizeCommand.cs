using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class UpdateDescriptionFontSizeCommand : ICommand
    {
        [Inject] private readonly HomeModel _homeModel;
        [Inject] private readonly AccessibilityModel _accessibilityModel;

        public void Execute()
        {
            foreach (var view in _homeModel.OpenedDescriptions)
            {
                view.Value.SetFontSize(_accessibilityModel.MainAppFontSizeScaler);
                view.Value.ResizeView();
            }
        }
    }

    public class UpdateDescriptionFontSizeCommandSignal : ISignal
    {
        
    }
}