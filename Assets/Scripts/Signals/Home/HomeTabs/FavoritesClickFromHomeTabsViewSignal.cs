
namespace TDL.Signals
{
	public class FavoritesClickFromHomeTabsViewSignal : ISignal
	{
		public bool Status { get; private set; }
    
		public FavoritesClickFromHomeTabsViewSignal(bool status)
		{
			Status = status;
		}
	}
}