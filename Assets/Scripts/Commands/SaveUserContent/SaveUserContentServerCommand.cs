using TDL.Services;
using TDL.Signals;
using Zenject;

namespace TDL.Commands
{
    public class SaveUserContentServerCommand : ICommandWithParameters
    {
        [Inject] private ServerService _serverService;

        public void Execute(ISignal signal)
        {
            var s = (SaveUserContentServerCommandSignal) signal;
            _serverService.SaveUserContent(s.ContentTypeId, s.AssetId, s.FileName, s.AssetName, s.FileBytes);
        }
    }
}

namespace TDL.Signals
{
    public class SaveUserContentServerCommandSignal : ISignal
    {
        public int ContentTypeId { get; private set; }
        public int AssetId { get; private set; }
        public string FileName { get; private set; }
        public string AssetName { get; private set; }
        public byte[] FileBytes { get; private set; }
    
        public SaveUserContentServerCommandSignal(int contentTypeId, int assetId, string fileName, string assetName, byte[] fileBytes)
        {
            ContentTypeId = contentTypeId;
            AssetId = assetId;
            FileName = fileName;
            AssetName = assetName;
            FileBytes = fileBytes;
        }
    }
}
