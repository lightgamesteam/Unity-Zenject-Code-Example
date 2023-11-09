using UnityEngine;
using Your.Namespace.Here.UniqueStringHereToAvoidNamespaceConflicts.Grids;

public class SaveMainScreenContentPanelsCommandSignal : ISignal
{
    public Transform AssetsContent { get; private set; }
    public MainScreenGridAdapter GridAdapter { get; private set; }

    public SaveMainScreenContentPanelsCommandSignal(Transform assetsContent, MainScreenGridAdapter gridAdapter)
    {
        AssetsContent = assetsContent;
        GridAdapter = gridAdapter;
    }
}