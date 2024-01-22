using System.Windows.Media;
using Config.Net;

namespace F3H.ProfileShark.Helpers;

public class ColorTypeParser : ITypeParser
{
    public bool TryParse(string? value, Type t, out object? result)
    {
        result = (Color)ColorConverter.ConvertFromString(value);
        return true;
    }

    public string? ToRawString(object? value)
    {
        return ((Color?)value)?.ToString();
    }

    public IEnumerable<Type> SupportedTypes  => new[] { typeof(Color) };
}