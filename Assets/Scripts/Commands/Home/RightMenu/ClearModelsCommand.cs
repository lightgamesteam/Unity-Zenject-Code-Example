using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ClearModelsCommand : ICommand
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private UserLoginModel _userLoginModel;

        public void Execute()
        {
            _userLoginModel.Reset();
            _homeModel.Reset();
            _contentModel.Reset();
        }
    }
}