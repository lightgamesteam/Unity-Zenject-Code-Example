using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class RemoveFromFavoritesCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (RemoveFromFavouritesCommandSignal) signal;
            _serverService.RemoveFromFavourites(parameter.GradeId, parameter.AssetId);
        }
    }
}