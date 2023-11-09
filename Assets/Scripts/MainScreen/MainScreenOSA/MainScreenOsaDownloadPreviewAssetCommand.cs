using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    public class MainScreenOsaDownloadPreviewAssetCommand : ICommandWithParameters
    {
        [Inject] protected readonly SignalBus _signal;

        public void Execute(ISignal signal)
        {
            var model = signal as MainScreenOsaInitializeAssetSignal;
            _signal.Fire(new DownloadThumbnailByIdSignal(model.AssetItem));
        }
    }
}