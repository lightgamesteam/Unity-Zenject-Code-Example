
namespace TDL.Signals
{
    public class PuzzleAssetItemClickViewSignal : ISignal
    {
        public int AssetId { get; private set; }
        public int GradeId { get; private set; }
        public int SelectedItemId { get; private set; }

        public PuzzleAssetItemClickViewSignal(int assetId, int gradeId, int selectedItemId)
        {
            AssetId = assetId;
            GradeId = gradeId;
            SelectedItemId = selectedItemId;
        }
    }
}