using System.IO;
using TDL.Models;
using TDL.Server;
using TDL.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Model3D
{
    public class LoadThumbnailsCommand : ICommandWithParameters
    {
        [Inject] private ContentModel _contentModel;
        [Inject] private ContentViewModel _contentViewModel;
        [Inject] private ICacheService _cacheService;
        [Inject] private ServerService _serverService;
        
        public void Execute(ISignal signal)
        {
            var parameter = (LoadThumbnailsCommandSignal) signal;

            switch (parameter.categoryOrAsset)
            {
                case Grade category:
                    break;
                
                case Subject category:
                    break;
                
                case Topic category:
                    break;
                
                case Subtopic category:
                    break;
                
                case Asset asset:
                    DownloadAssetThumbnail(asset.Id, parameter.image);

                    break;
            }
        }

        private void DownloadAssetThumbnail(int id, Image itemView)
        {
            var itemModel = _contentModel.GetAssetById(id);
            var thumbnailName =  GetThumbnailName(itemModel.Asset.ThumbnailUrl);

            if (!string.IsNullOrEmpty(thumbnailName))
            {
                if (_cacheService.IsFileExists(thumbnailName))
                {
                    SetThumbnail(itemView, thumbnailName);
                }
                else
                {
                    var thumbnailUrl = itemModel.Asset.ThumbnailUrl;
                    _serverService.DownloadThumbnail(id.ToString(),thumbnailName, thumbnailUrl,
                        (isDownload, textureName) =>
                        {
                            if (isDownload)
                            {
                                if(itemView != null)
                                    SetThumbnail(itemView, textureName);
                            }
                        });
                }
            }
        }

        private void SetThumbnail(Image itemView, string thumbnailName)
        {
            if(!_cacheService.IsFileExists(thumbnailName))
                return;
            
            var thumbnailPath = _cacheService.GetPathToFile(thumbnailName);
            var texture = new Texture2D(1, 1);
            texture.LoadImage(File.ReadAllBytes(thumbnailPath));
            
            _contentViewModel.CachedTextures.Add(texture);
            
            itemView.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        
        private string GetThumbnailName(string url)
        {
            return Path.GetFileName(url);
        }
    }

    public class LoadThumbnailsCommandSignal : ISignal
    {
        public object categoryOrAsset;
        public Image image;

        public LoadThumbnailsCommandSignal(object _categoryOrAsset, Image _image)
        {
            categoryOrAsset = _categoryOrAsset;
            image = _image;
        }
    }
}