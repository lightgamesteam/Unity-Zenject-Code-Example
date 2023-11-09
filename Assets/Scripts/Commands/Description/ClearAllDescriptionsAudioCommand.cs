using TDL.Models;
using Zenject;

namespace TDL.Commands
{
    public class ClearAllDescriptionsAudioCommand : ICommand
    {
        [Inject] private HomeModel _homeModel;

        public void Execute()
        {
            foreach (var description in _homeModel.OpenedDescriptions)
            {
                description.Value.ClearAudioSource();
            }
        }
    }
}