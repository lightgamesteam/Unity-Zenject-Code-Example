using Signals;
using System.Collections.Generic;
using System.IO;
using TDL.Models;
using TDL.Services;
using UnityEngine;
using Zenject;

namespace Commands
{
    public class CreateThumbnailByIdCommand : ICommandWithParameters
    {
        [Inject] protected ContentModel _contentModel;
        [Inject] protected UserContentAppModel _userContentAppModel;
        [Inject] protected HomeModel _homeModel;
        [Inject] protected ICacheService _cacheService;
        [Inject] protected ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var model = signal as DownloadThumbnailByIdSignal;
            DownloadAssetThumbnail(model.Asset.AssetId, model.Asset);
        }

        protected void DownloadAssetThumbnail(int id, IThumbnailView itemView)
        {
            var itemModel = _contentModel.GetAssetById(id);
            var thumbnailName = GetThumbnailName(itemModel.Asset.ThumbnailUrl);

            if (!string.IsNullOrEmpty(thumbnailName))
            {
                if (IsFileAlreadyInCache(thumbnailName))
                {
                    itemView.SetThumbnail(CreateThumbnailFromCache(thumbnailName));
                }
                else
                {
                    var thumbnailUrl = itemModel.Asset.ThumbnailUrl;
                    if (!string.IsNullOrEmpty(thumbnailUrl))
                    {
                        DownloadThumbnailFromServer(id.ToString(), thumbnailName, thumbnailUrl, itemView);
                    }
                }
            }
        }

        private string GetThumbnailName(string url)
        {
            return Path.GetFileName(url);
        }

        private bool IsFileAlreadyInCache(string fileName)
        {
            return _cacheService.IsFileExists(fileName);
        }

        private void DownloadThumbnailFromServer(string thumbnailId, string thumbnailName, string thumbnailUrl, IThumbnailView itemView)
        {
            if (_homeModel.ThumbnailsToDownload.ContainsKey(thumbnailId))
            {
                var thumbnail = _homeModel.ThumbnailsToDownload[thumbnailId];
                thumbnail.Add(itemView);
            }
            else
            {
                _serverService.DownloadThumbnail(thumbnailId, thumbnailName, thumbnailUrl);
                _homeModel.ThumbnailsToDownload.Add(thumbnailId, new List<IThumbnailView> { itemView });
            }
        }

        private Texture2D CreateThumbnailFromCache(string fileName)
        {
            var thumbnailPath = _cacheService.GetPathToFile(fileName);
            var texture2D = new Texture2D(1, 1);
            texture2D.LoadImage(File.ReadAllBytes(thumbnailPath));

            _contentModel.CachedTextures.Add(texture2D);

            return texture2D;
        }
    }
}