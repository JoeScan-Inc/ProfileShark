using System.Drawing.Imaging;
using Caliburn.Micro;

// ReSharper disable UnusedMember.Global

namespace F3H.ProfileShark.RawBoard3D;

// ReSharper disable once ClassNeverInstantiated.Global
public class DisplaySettingsViewModel : Screen
{
    private readonly I3DDisplaySettings settings;

    public DisplaySettingsViewModel(I3DDisplaySettings settings)
    {
        this.settings = settings;
        this.settings.PropertyChanged += (_, _) => NotifyOfPropertyChange(string.Empty);
    }

    public bool ShowRawPoints
    {
        get => settings.ShowRawPoints;
        set => settings.ShowRawPoints = value;
    }


    public bool ShowDebugStuff
    {
        get => settings.ShowDebugStuff;
        set => settings.ShowDebugStuff = value;
    }


    public DisplayMode DisplayMode
    {
        get => settings.DisplayMode;
        set => settings.DisplayMode = value;
    }

    public double RawPointSize
    {
        get => settings.RawPointSize;
        set => settings.RawPointSize = value;
    }


    public bool OrthographicCamera 
    {
        get => settings.OrthographicCamera;
        set => settings.OrthographicCamera = value;
    }
    
    public void SaveSettings()
    {
        
    }
}