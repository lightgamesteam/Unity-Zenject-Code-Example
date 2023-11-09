using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class GetActivityContentCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (GetActivityContentCommandSignal) signal;

            foreach (var activity in parameter.ActivityIds)
            {
                _serverService.GetActivity(activity.Key, activity.Value);
            }
        }
    }
}
