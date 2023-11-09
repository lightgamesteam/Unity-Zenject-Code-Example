using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class GetTeacherContentCommand : ICommand
    {
        [Inject] private ServerService _serverService;

        public void Execute()
        {
            _serverService.GetTeacherContent();
        }
    }
}


namespace TDL.Signals
{
    public class GetTeacherContentCommandSignal : ISignal
    {
        
    }
}