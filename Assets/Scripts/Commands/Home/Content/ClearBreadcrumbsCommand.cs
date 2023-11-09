using TDL.Views;
using Zenject;

namespace TDL.Commands
{
    public class ClearBreadcrumbsCommand : ICommand
    {
        [InjectOptional] private HomeViewMediator _homePCViewMediator;
        [InjectOptional] private HomeViewMobileMediator _homeMobileViewMediator;

        public void Execute()
        {
            if (DeviceInfo.IsPCInterface())
            {
                _homePCViewMediator.ResetAllBreadcrumbs();
            }
            else
            {
                _homeMobileViewMediator.ResetAllBreadcrumbs();
            }
        }
    }
}