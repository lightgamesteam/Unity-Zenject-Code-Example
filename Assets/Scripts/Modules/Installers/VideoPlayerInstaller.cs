using TDL.Modules.Model3D;
using Zenject;

namespace TDL.Core
{
    public class VideoPlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VideoPlayerMediator>().AsSingle();
        }
    }
}