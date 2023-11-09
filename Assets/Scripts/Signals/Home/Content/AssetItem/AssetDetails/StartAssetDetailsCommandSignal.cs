using System.Collections.Generic;

namespace TDL.Signals
{
    public class StartAssetDetailsCommandSignal : ISignal
    {
        public List<(int AssetId, int GradeId)> Ids { get; private set; }

        public StartAssetDetailsCommandSignal(List<(int AssetId, int GradeId)>  ids)
        {
            Ids = ids;
        }
        
        public StartAssetDetailsCommandSignal(List<int> ids)
        {
            Ids = new List<(int AssetId, int GradeId)>();
            ids.ForEach(id =>
            {
                Ids.Add((id, -1));
            });
        }
    }
}