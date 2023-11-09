using System.IO;
using TDL.Models;
using TDL.Services;
using TDL.Signals;
using UnityEngine;
using Zenject;

namespace TDL.Commands
{
    public class SaveThumbnailInCacheCommand : ICommandWithParameters
    {
        [Inject] private ICacheService _cacheService;
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveThumbnailInCacheCommandSignal) signal;
            Debug.Log($"Save in cache for Id: {parameter.ItemId}");

            var pathToSave = _cacheService.GetPathToFile(parameter.ItemName);
            using (var fs = new FileStream(pathToSave, FileMode.OpenOrCreate))
            {
                fs.Write(parameter.DataResponse, 0, parameter.DataResponse.Length);
            }
        }
    }
}