using UnityEngine.UI;

public class TakeScreenshotSignal : ISignal
{
   public int AssetID { get; private set; }
   public string CultureCore { get; private set; }
   
   public TakeScreenshotSignal( int assetID, string cultureCore = "")
   {
      AssetID = assetID;
      CultureCore = cultureCore;
   }
}
