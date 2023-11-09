
namespace TDL.Modules.Model3D
{
    public class Module3dCreateAssetDetailsCommandSignal : ISignal
    {
        public int Id { get; private set; }
        public string Response { get; private set; }

        public Module3dCreateAssetDetailsCommandSignal(int id, string response)
        {
            Id = id;
            Response = response;
        }
    }
}