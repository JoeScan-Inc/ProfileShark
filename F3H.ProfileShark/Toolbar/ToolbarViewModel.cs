using Caliburn.Micro;
using F3H.ProfileShark.Models;
using F3H.ProfileShark.Shared;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using NLog;

namespace F3H.ProfileShark.Toolbar;

public class ToolbarViewModel : Screen
{
    private readonly IDialogService dialogService;
    public DataManager Data { get; }
    public IEventAggregator EventAggregator { get; }
    public ILogger Logger { get; }

    public ToolbarViewModel(DataManager data,
       IEventAggregator eventAggregator,
        IDialogService dialogService, ILogger logger)
    {
        this.dialogService = dialogService;
        Data = data;
        EventAggregator = eventAggregator;
        Logger = logger;
    }

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
            try
            {
                EventAggregator.PublishOnUIThreadAsync(true);
                Data.SetProfiles(await DataReader.ReadProfilesC(openFileDialogSettings.FileName));
            }
            catch (Exception e)
            {

            }
            finally
            {
                EventAggregator.PublishOnUIThreadAsync(false);
            }
           
        }
    }
   

}