
namespace TDL.Modules.Model3D
{
    public class Module3dProcessAssetDetailsCommandSignal : ISignal
    {
        public int Id { get; private set; }

        public Module3dProcessAssetDetailsCommandSignal(int id)
        {
            Id = id;
        }
    }
}