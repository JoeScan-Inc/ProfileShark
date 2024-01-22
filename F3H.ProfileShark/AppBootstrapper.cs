using System.IO;
using System.Windows;
using Autofac;
using Autofac.Extras.NLog;
using Config.Net;
using F3H.ProfileShark.Helpers;
using F3H.ProfileShark.Models;
using F3H.ProfileShark.RawBoard3D;
using F3H.ProfileShark.Shell;
using MvvmDialogs;

namespace F3H.ProfileShark;

public class AppBootstrapper : AutofacBootstrapper
{
    protected override void OnStartup(object sender, StartupEventArgs e)
    {
#pragma warning disable CA1416
        DisplayRootViewForAsync<ShellViewModel>();
#pragma warning restore CA1416
    }

    protected override void OnExit(object sender, EventArgs e)
    {
        Container.Dispose();
    }

    protected override void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<NLogModule>();
        builder.RegisterType<DataManager>().AsSelf().SingleInstance();
        builder.RegisterType<DialogService>().As<IDialogService>();
        builder.RegisterType<PlotColorService>().AsSelf().SingleInstance();
        builder.RegisterType<ItemColorService>().AsSelf().SingleInstance();
        builder.Register(c =>
        {
            var loc = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var name = Path.Combine(loc, "Live3DViewSettings.json");
            JsonHelper.CreateEmptyFileIfNotExisting(name);
            return new ConfigurationBuilder<I3DDisplaySettings>()
                .UseTypeParser(new ColorTypeParser()).UseJsonFile(name).Build();

        }).SingleInstance();
    }
}