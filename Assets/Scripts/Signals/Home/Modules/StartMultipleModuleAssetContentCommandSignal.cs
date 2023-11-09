
namespace TDL.Signals
{
    public class StartMultipleModuleAssetContentCommandSignal : ISignal
    {
        public string ModuleName { get; private set; }
        public string LoadingMessage { get; private set; }

        public StartMultipleModuleAssetContentCommandSignal(string moduleName, string loadingMessage)
        {
            ModuleName = moduleName;
            LoadingMessage = loadingMessage;
        }
    }
}