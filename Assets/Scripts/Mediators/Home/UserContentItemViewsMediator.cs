using Zenject;
using System.IO;
using TDL.Models;
using TDL.Services;
using TDL.Signals;

namespace TDL.Views
{
    public class UserContentItemViewsMediator : IInitializable
    {
        [Inject] private readonly SignalBus _signal;
        [Inject] private HomeModel _homeModel;
        [Inject] private ServerService _serverService;
        [Inject] private readonly UserContentAppModel _userContentAppModel;
        [Inject] private UserContentItemView.Pool _pool;

        public void Initialize()
        {
            SubscribeOnListeners();
        }

        private void SubscribeOnListeners()
        {
            _signal.Subscribe<UserContentItemClickViewSignal>(OnAssetItemClick);
            _signal.Subscribe<UserContentItemDeleteClickViewSignal>(OnAssetItemDeleteClick);
        }

        private void OnAssetItemClick(UserContentItemClickViewSignal signal)
        {
            HideSideMenus();

            _signal.Fire(new StartUserContentViewerModuleCommandSignal(signal.Id));
        }

        private void OnAssetItemDeleteClick(UserContentItemDeleteClickViewSignal signal)
        {
            _serverService.DeleteUserContent(signal.UserContentItem.Id);
            
            _pool.Despawn(signal.UserContentItem);
            _homeModel.ShownUserContentOnHome.Remove(signal.UserContentItem);

            var userContent = _userContentAppModel.GetUserContentById(signal.UserContentItem.Id);

            if (File.Exists(userContent.FilePath))
            {
                File.Delete(userContent.FilePath);
            }
            
            _userContentAppModel.UserContentList.RemoveAll(uc => uc.Id == signal.UserContentItem.Id);
        }

        private void HideSideMenus()
        {
            _signal.Fire(new ShowLeftMenuCommandSignal(false));
            _signal.Fire(new ShowRightMenuCommandSignal(false));
        }
    }
}