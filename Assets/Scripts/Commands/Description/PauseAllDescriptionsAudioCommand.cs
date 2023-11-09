using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class PauseAllDescriptionsAudioCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            foreach (var description in _homeModel.OpenedDescriptions)
            {
                description.Value.PlaySound(false);
            }
        }
    }

    public class PauseAllDescriptionsAudioCommandSignal : ISignal
    {
        
    }
}