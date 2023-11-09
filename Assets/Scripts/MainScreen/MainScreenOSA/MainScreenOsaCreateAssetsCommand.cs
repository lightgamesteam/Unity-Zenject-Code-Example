using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DG.Tweening;
using Signals;
using TDL.Constants;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;
using static TDL.Models.HomeModel;

namespace TDL.Commands
{
    public class MainScreenOsaCreateAssetsCommand
    {
        [Inject] private MainScreenModel _screenModel;
        [Inject] private readonly ContentModel _contentModel;
        [Inject] private readonly ApplicationSettingsInstaller.AssetItemResources _assetItemResources;
        [Inject] private readonly UserLoginModel _userLoginModel;

        private List<ClientAssetModel> foundedAssets;

        public void Execute()
        {
            ClearThumbnails();

            var isTeacher = _userLoginModel.IsTeacher;

            foundedAssets = GetAssets();
            DisableAsset(foundedAssets, AssetTypeConstants.Type_Module, ModuleConstants.Module_Astronomy);

            Debug.Log("Add items");
            DOVirtual.DelayedCall(1.5f, AddItems);
        }

        protected void ClearThumbnails()
        {
            _screenModel.ThumbnailsToDownload.Clear();
        }

        private List<ClientAssetModel> GetAssets()
        {
            return _contentModel.GetAllAvailableAssets();
        }

        private void DisableAsset(List<ClientAssetModel> assetModels, string assetType, string assetName)
        {
            if (DeviceInfo.IsChromebook())
            {
                assetModels.RemoveAll(clientAsset =>
                    clientAsset.Asset.Type.ToLower().Equals(assetType)
                    && clientAsset.Asset.Name.Equals(assetName));
            }
        }

        private void AddItems()
        {
            Debug.Log("Set items");
            Debug.Log(_screenModel.GridAdapter == null);
            _screenModel.GridAdapter.SetItems(foundedAssets);
        }
    }
}