using System.Collections.Generic;

public class Content3DModelHistory
{
    private List<int> _historyContentID = new List<int>();

    public void AddHistory(int id)
    {
        _historyContentID.Add(id);
    }
    
    public void ClearHistory()
    {
       _historyContentID.Clear();
    }

    public void UndoHistory()
    {
       RemoveLastItem();
       RemoveLastItem();
    }

    public int GetLastItem()
    {
        return _historyContentID[GetLastIndex()];
    }
    
    public void RemoveLastItem()
    {
        if (ContainItems())
            _historyContentID.RemoveAt(GetLastIndex());
    }
    
    int GetLastIndex()
    {
        return _historyContentID.Count - 1;
    }
    
    public bool ContainItems()
    {
        return _historyContentID.Count > 0;
    }
}
