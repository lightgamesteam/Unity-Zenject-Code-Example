using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class AddToFavoritesCommand : ICommandWithParameters
    {
        [Inject] private readonly ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (AddToFavouritesCommandSignal) signal;

            _serverService.AddToFavourites(parameter.GradeId, parameter.AssetId);
        }
    }
}