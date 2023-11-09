
namespace TDL.Signals
{
	public class SaveRightMenuItemViewSignal : ISignal
	{
		public string MenuItemName { get; private set; }
		public ISelectableMenuItem MenuItem { get; private set; }
    
		public SaveRightMenuItemViewSignal(string menuItemName, ISelectableMenuItem menuItem)
		{
			MenuItemName = menuItemName;
			MenuItem = menuItem;
		}
	}
}