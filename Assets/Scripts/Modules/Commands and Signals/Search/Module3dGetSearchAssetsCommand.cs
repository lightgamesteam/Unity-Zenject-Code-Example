using Zenject;

namespace TDL.Modules.Model3D
{
    public class Module3dGetSearchAssetsCommand : ICommandWithParameters
    {
        [Inject] private readonly IModule3dServerService _server;

        public void Execute(ISignal signal)
        {
            var parameter = (Module3dGetSearchAssetsCommandSignal) signal;

            _server.GetSearch(parameter.SearchValue, parameter.CultureCode, parameter.AssetTypes, parameter.ClientAssetModels);
        }
    }
}