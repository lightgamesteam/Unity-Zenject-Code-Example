using System;
using System.IO;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using TDL.Views;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SetThumbnailForViewCommand : ICommandWithParameters
    {
        [Inject] private ICacheService _cacheService;
        [Inject] private HomeModel _homeModel;
        [Inject] private ContentModel _contentModel;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveThumbnailInCacheCommandSignal)signal;

            if (_homeModel.ThumbnailsToDownload.ContainsKey(parameter.ItemId))
            {
                var thumbnailPath = _cacheService.GetPathToFile(parameter.ItemName);
                var texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(File.ReadAllBytes(thumbnailPath));

                _contentModel.CachedTextures.Add(texture2D);

                var itemViews = _homeModel.ThumbnailsToDownload[parameter.ItemId];

                foreach (var view in itemViews)
                {
                    // var assetView = (AssetItemView)view;
                    // if (assetView.AssetId != int.Parse(parameter.ItemId))
                    //     continue;

                    view.SetThumbnail(texture2D);
                }
            }
        }
    }
}