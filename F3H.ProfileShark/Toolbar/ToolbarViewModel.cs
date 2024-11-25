using System.Dynamic;
using System.IO;
using System.Windows;
using Caliburn.Micro;
using F3H.ProfileShark.Dialogs;
using F3H.ProfileShark.Models;
using F3H.ProfileShark.Shared;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using NLog;

namespace F3H.ProfileShark.Toolbar;

public class ToolbarViewModel : Screen
{
    private readonly IDialogService dialogService;
    private readonly IWindowManager windowManager;

    public ToolbarViewModel(DataManager dataManager,
       IEventAggregator eventAggregator,
        IDialogService dialogService, IWindowManager windowManager, ILogger logger)
    {
        this.dialogService = dialogService;
        this.windowManager = windowManager;
        DataManager = dataManager;
        EventAggregator = eventAggregator;
        Logger = logger;
    }

    public DataManager DataManager { get; }
    public IEventAggregator EventAggregator { get; }
    public ILogger Logger { get; }

    public bool CanLoadNext => !String.IsNullOrEmpty(DataManager.CurrentFile);
    public bool CanLoadPrevious => !String.IsNullOrEmpty(DataManager.CurrentFile);

    public async void Load()
    {
        // var initialDirectory =  Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        var openFileDialogSettings = new OpenFileDialogSettings()
        {
            Title = "Open raw log archive file",
            // InitialDirectory = initialDirectory,
            Filter = $"Raw Recording (*.bin)|*.bin|All Files (*.*)|*.*",
            CheckFileExists = true
        };

        bool? success = dialogService.ShowOpenFileDialog(this, openFileDialogSettings);
        if (success == true)
        {
            await LoadFile(openFileDialogSettings.FileName);
        }
    }

    private async Task LoadFile(string fileName)
    {
        try
        {
            await EventAggregator.PublishOnUIThreadAsync(true);
            DataManager.SetProfiles(await DataReader.ReadProfilesC(fileName));
            DataManager.CurrentFile = fileName;
            Refresh();
        }
        catch (Exception e)
        {
            // ignored
        }
        finally
        {
            await EventAggregator.PublishOnUIThreadAsync(false);
        }
    }

    public async Task Record()
    {
        try
        {
            await EventAggregator.PublishOnUIThreadAsync(true);
            // show recording dialog
            var dlg = IoC.Get<RecordLiveDataViewModel>();
            dynamic dlgSettings = new ExpandoObject();
            dlgSettings.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dlgSettings.ShowInTaskbar = false;
            await windowManager.ShowDialogAsync(dlg, null, dlgSettings);
            
        }
        catch (Exception e)
        {
            // ignored
        }
        finally
        {
            await EventAggregator.PublishOnUIThreadAsync(false);
        }
    }

    public async void LoadPrevious()
    {
        await LoadFile(GetNextFile(-1));
    }

    public async void LoadNext()
    {
        await LoadFile(GetNextFile(1));
    }

    private string GetNextFile(int offset)
    {
        var dir = Path.GetDirectoryName(DataManager.CurrentFile);
        var files = Directory.GetFiles(dir, "*.bin");
        var index = Array.IndexOf(files, DataManager.CurrentFile);
        if (index == -1)
        {
            return "";
        }
        // get file at index + offset
        return files[(index + offset) % files.Length];
    }
}