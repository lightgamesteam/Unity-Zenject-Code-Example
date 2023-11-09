
namespace TDL.Signals
{
	public class MainScreenStartModuleAssetContentCommandSignal : ISignal
	{
		public string ModuleName { get; private set; }
		public string LoadingMessage { get; private set; }

		public int AssetId { get; set; }

		public MainScreenStartModuleAssetContentCommandSignal(int assetId, string moduleName, string loadingMessage)
		{
			AssetId = assetId;
			ModuleName = moduleName;
			LoadingMessage = loadingMessage;
		}
	}
}