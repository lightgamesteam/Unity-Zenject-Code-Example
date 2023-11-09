using System.IO;
using TDL.Models;
using TDL.Services;
using Zenject;

namespace TDL.Commands
{
    public class SaveBackgroundInCacheCommand : ICommandWithParameters
    {
        [Inject] private ICacheService _cacheService;
        [Inject] private HomeModel _homeModel;

        public void Execute(ISignal signal)
        {
            var parameter = (SaveBackgroundInCacheCommandSignal) signal;

            var pathToSave = _cacheService.GetPathToFile(parameter.ItemName);
            using (var fs = new FileStream(pathToSave, FileMode.OpenOrCreate))
            {
                fs.Write(parameter.DataResponse, 0, parameter.DataResponse.Length);
            }
        }
    }
}