
public class ChangeLanguageCommandSignal : ISignal
{
    public int LanguageIndex { get; private set; }

    public ChangeLanguageCommandSignal(int languageIndex)
    {
        LanguageIndex = languageIndex;
    }
}