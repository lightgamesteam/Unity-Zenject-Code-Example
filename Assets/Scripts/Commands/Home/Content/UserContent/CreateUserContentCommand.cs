using TDL.Constants;
using TDL.Models;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class CreateUserContentCommand : ICommandWithParameters
    {
        [Inject] private HomeModel _homeModel;
        [Inject] private readonly UserContentAppModel _userContentAppModel;
        [Inject] private readonly ApplicationSettingsInstaller.AssetItemResources _assetItemResources;
        [Inject] private readonly UserContentItemView.Pool _assetPool;

        public void Execute(ISignal signal)
        {
            var foundedAssets = _userContentAppModel.GetUserContentList();
            
            foreach (var assetModel in foundedAssets)
            {
                var assetView = _assetPool.Spawn(_homeModel.AssetsContent);
                
                _homeModel.ShownUserContentOnHome.Add(assetView);
                assetView.transform.SetParent(_homeModel.AssetsContent, false);
                assetView.transform.SetAsLastSibling();
                assetView.Id = assetModel.Id;
                assetView.TypeId = assetModel.ContentTypeId;
                assetView.Title.text = assetModel.Name;
                assetView.SetAssetType(GetAssetType(assetModel.ContentTypeId));

                if (DeviceInfo.IsPCInterface())
                {
                    SetTooltip(assetView);
                }
            }
        }

        private void SetTooltip(UserContentItemView assetView)
        {
            var tooltip = assetView.gameObject.GetComponent<DynamicTooltipEvents>();
            tooltip.SetHint(assetView.Title.text);
        }

        private Sprite GetAssetType(int assetType)
        {
            switch (assetType)
            {
                case UserContentTypeIDConstants.Image:
                    return _assetItemResources.TypeImage;

                case UserContentTypeIDConstants.Video:
                    return _assetItemResources.Type2DVideo;

                default:
                    return null;
            }
        }
    }
}