using System.Collections;
using System.Collections.Generic;
using TDL.Models;
using TDL.Views;
using UnityEngine;

public class MainScreenOsaInitializeAssetSignal : ISignal
{
    public WebAssetItemView AssetItem { get; private set; }
    public ClientAssetModel ClientModel { get; private set; }

    public MainScreenOsaInitializeAssetSignal(WebAssetItemView assetItem, ClientAssetModel clientModel)
    {
        AssetItem = assetItem;
        ClientModel = clientModel;
    }
}