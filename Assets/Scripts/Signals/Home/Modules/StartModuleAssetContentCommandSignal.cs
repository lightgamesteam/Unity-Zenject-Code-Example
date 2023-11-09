
namespace TDL.Signals
{
	public class StartModuleAssetContentCommandSignal : ISignal
	{
		public string ModuleName { get; private set; }
		public string LoadingMessage { get; private set; }

		public StartModuleAssetContentCommandSignal(string moduleName, string loadingMessage)
		{
			ModuleName = moduleName;
			LoadingMessage = loadingMessage;
		}
	}
}