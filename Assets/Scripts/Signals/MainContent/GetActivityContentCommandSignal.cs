using System.Collections.Generic;

namespace TDL.Signals
{
    public class GetActivityContentCommandSignal : ISignal
    {
        public Dictionary<string, int> ActivityIds { get; private set; }

        public GetActivityContentCommandSignal(Dictionary<string, int> activityIds)
        {
            ActivityIds = activityIds;
        }
    }
}