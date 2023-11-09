using UnityEngine.UI;

public class StartVideoRecordingSignal : ISignal
{
   public bool IsRecording { get; private set; }
   public int AssetID { get; private set; }
   public Toggle ToggleVideoRecording { get; private set; }
   public string CultureCore { get; private set; }
   
   public StartVideoRecordingSignal(bool isRecording, int assetID, Toggle toggleVideoRecording, string cultureCore = "")
   {
      IsRecording = isRecording;
      AssetID = assetID;
      ToggleVideoRecording = toggleVideoRecording;
      CultureCore = cultureCore;
   }
}
