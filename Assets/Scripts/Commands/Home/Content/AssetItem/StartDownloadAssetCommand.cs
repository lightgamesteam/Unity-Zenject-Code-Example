using System.IO;
using TDL.Models;
using TDL.Services;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class StartDownloadAssetCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private HomeModel _homeModel;
        [Inject] private ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var parameter = (StartDownloadAssetCommandSignal) signal;
            var assetId = parameter.Id;

            var assetModel = _contentModel.GetAssetById(assetId);

            var assetContent = assetModel.AssetDetail.AssetContentPlatform;
            var assetUrl = assetContent.FileUrl;
            var assetName = Path.GetFileName(assetContent.FileUrl);

            Debug.Log("StartDownloadAssetCommand => assetName = " + assetName);
            Debug.Log("StartDownloadAssetCommand => assetUrl = " + assetUrl);
            Debug.Log("StartDownloadAssetCommand => assetModel.Asset.Version = " + assetModel.Asset.Version);
            var downloadRequest = _serverService.DownloadAsset(assetId, assetUrl);

            var downloadItem = new HomeModel.DownloadAsset
            {
                Id = assetId,
                Version = assetModel.Asset.Version,
                Name = assetName,
                ItemRequest = downloadRequest,
                Progress = 0
            };

            _homeModel.AssetsToDownload.Add(downloadItem);

            downloadRequest.Send();
        }
    }
}