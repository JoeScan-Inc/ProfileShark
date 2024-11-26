using System.Configuration;
using System.Data;
using System.Windows;

namespace F3H.ProfileShark;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        Logging.LogConfigurator.ConfigureLogging(e.Args);
        base.OnStartup(e);
    }
}

