using System;
using TDL.Server;

public static class ConverterExtension
{
    public static LocalName[] ConvertToLocalName(this LabelLocalName[] input)
    {
        return Array.ConvertAll(input, item => new LocalName() {Culture = item.Culture, Name = item.Name});
    }
}
