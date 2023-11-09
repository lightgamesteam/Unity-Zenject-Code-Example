public class VideoRecordingStateSignal : ISignal
{
   public RecordingState State { get; private set; }
   
   public VideoRecordingStateSignal(RecordingState state)
   {
      State = state;
   }
}
