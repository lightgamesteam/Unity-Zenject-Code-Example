using TDL.Views;

public class UserContentItemDeleteClickViewSignal : ISignal
{
    public UserContentItemView UserContentItem { get; private set; }

    public UserContentItemDeleteClickViewSignal(UserContentItemView userContentItem)
    {
        UserContentItem = userContentItem;
    }
}