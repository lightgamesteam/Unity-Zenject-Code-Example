using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class RegisterScreenCommand : ICommandWithParameters
    {
        [Inject] private IWindowService _windowService;

        public void Execute(ISignal signal)
        {
            var parameter = (RegisterScreenCommandSignal) signal;
            _windowService.AddWindow(parameter.ScreenName, parameter.ScreenObject);
        }
    } 
}