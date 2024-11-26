using System.Windows;

namespace F3H.ProfileShark.Converters;

public sealed class BoolToVisibilityConverter : BooleanConverter<Visibility>
{
    public BoolToVisibilityConverter() :
        base(Visibility.Visible, Visibility.Collapsed)
    { }
}
