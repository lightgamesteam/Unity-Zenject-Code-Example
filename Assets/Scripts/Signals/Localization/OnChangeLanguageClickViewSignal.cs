
public class OnChangeLanguageClickViewSignal : ISignal
{
    public int LanguageIndex { get; private set; }

    public OnChangeLanguageClickViewSignal(int languageIndex)
    {
        LanguageIndex = languageIndex;
    }
}