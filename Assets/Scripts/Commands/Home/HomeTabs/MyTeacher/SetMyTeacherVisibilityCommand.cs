using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class SetMyTeacherVisibilityCommand : ICommand
    {
        [Inject] private readonly UserLoginModel _userLoginModel;
        [Inject] private readonly HomeModel _homeModel;

        public void Execute()
        {
            _homeModel.HomeTabMyTeacherVisible = !_userLoginModel.IsTeacher;
        }
    }
}