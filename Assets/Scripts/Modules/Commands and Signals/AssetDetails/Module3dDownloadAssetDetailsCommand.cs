using Zenject;

namespace TDL.Modules.Model3D
{
        public class Module3dDownloadAssetDetailsCommand : ICommandWithParameters
        {
            [Inject] private readonly IModule3dServerService _server;
    
            public void Execute(ISignal signal)
            {
                var parameter = (Module3dDownloadAssetDetailsCommandSignal) signal;

                _server.GetAssetDetails(parameter.Id);
            }
        }
}