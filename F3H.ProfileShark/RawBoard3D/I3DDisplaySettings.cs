using System.ComponentModel;
using System.Windows.Media;
using Config.Net;

namespace F3H.ProfileShark.RawBoard3D;

public interface I3DDisplaySettings : INotifyPropertyChanged
{
    [Option(DefaultValue = true)] bool ShowRawPoints { get; set; }
    [Option(DefaultValue = true)] bool ShowDebugStuff { get; set; }
    [Option(DefaultValue = "ByIntensity")] DisplayMode DisplayMode { get; set; }
    [Option(DefaultValue = "2.0")] double RawPointSize { get; set; }
    [Option(DefaultValue = "#ffda00")] Color CameraAColor { get; set; }
    [Option(DefaultValue = "Purple")] Color CameraBColor { get; set; }
    [Option(DefaultValue = false)] bool OrthographicCamera { get; set; }
}