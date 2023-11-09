
namespace TDL.Modules.Model3D
{
    public class Module3dStartAssetDetailsCommandSignal : ISignal
    {
        public int Id { get; private set; }

        public Module3dStartAssetDetailsCommandSignal(int id)
        {
            Id = id;
        }
    }
}