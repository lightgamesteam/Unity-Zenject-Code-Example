
namespace TDL.Modules.Model3D
{
    public class Module3dDownloadAssetDetailsCommandSignal : ISignal
    {
        public int Id { get; private set; }

        public Module3dDownloadAssetDetailsCommandSignal(int id)
        {
            Id = id;
        }
    }
}