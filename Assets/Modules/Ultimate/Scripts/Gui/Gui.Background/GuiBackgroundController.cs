using System.IO;
using DG.Tweening;
using Gui.Core;
using TDL.Models;
using TDL.Modules.Model3D;
using TDL.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace TDL.Modules.Ultimate.GuiBackground {
    public class GuiBackgroundController : GuiControllerBase {
        [Inject] private readonly ContentModel _contentModel = default;
        [Inject] private readonly SignalBus _signal = default;
        [Inject] private readonly ICacheService _cacheService = default;
        
        [Inject(Id = "Background")] 
        private readonly RawImage _backgroundRawImage = default;
        
        public override void Initialize() {
            base.Initialize();
            if (!_contentModel.HasBackground(_contentModel.SelectedAsset.Asset.Id)) {
                SetActiveBackground(false);
                return;
            }

            _signal.Fire(new DownloadBackgroundCommandSignal(_contentModel.SelectedAsset.Asset.Id, (isDownloaded, id) => {
                if (!isDownloaded) { return; }
                
                var thumbnailPath = _cacheService.GetPathToFile(Path.GetFileName(_contentModel.GetAssetById(id).AssetDetail.AssetContentPlatform.BackgroundUrl));
                    var texture2D = new Texture2D(1, 1);
                    texture2D.LoadImage(File.ReadAllBytes(thumbnailPath));
                    
                    SetActiveBackground(true);
                    _backgroundRawImage.texture = texture2D;
                    _backgroundRawImage.color = Color.clear;
                    _backgroundRawImage.DOColor(Color.white, 1f);
            }));
        }

        public void SetActiveBackground(bool isActive) {
            _backgroundRawImage.gameObject.SetActive(isActive);
        }
    }
}