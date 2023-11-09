public class ShowStudentNotesPanelViewSignal : ISignal
{
    public int AssetID { get;}
    public string CultureCode { get;}

    public ShowStudentNotesPanelViewSignal(int assetID, string cultureCode = null)
    {
        AssetID = assetID;
        CultureCode = cultureCode;
    }
}