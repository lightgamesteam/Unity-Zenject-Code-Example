public class SelectableLayerV4 : SelectableLayerAbstract
{
	public int startIndex;
	public override SelectableLayerElements GetLayer()
	{
		selectablesLayer.CurrentSelectableIndex = startIndex;
		return selectablesLayer;
	}
}