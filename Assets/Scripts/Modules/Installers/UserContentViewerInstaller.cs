using TDL.Commands;
using TDL.Modules.UserContentViewer;
using TDL.Signals;
using Zenject;

namespace TDL.Core
{
    public class UserContentViewerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserContentViewerMediator>().AsSingle();
        }
    }
}