using Caliburn.Micro;
using JoeScan.Pinchot;
using JoeScan.Pinchot.Parser;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using NLog;

namespace F3H.ProfileShark.Dialogs;

public class RecordLiveDataViewModel : Screen
{
    private readonly ILogger logger;
    private readonly IDialogService dialogService;

    #region Private Fields

    private bool isRecording;
    private uint minScanPeriod = 1000;
    private string outputFileName = string.Empty;
    private string scanSystemFileName = string.Empty;
    ScanSystem? scanSystem = null;
    private string message;

    #endregion

    #region Lifecycle

    public RecordLiveDataViewModel(ILogger logger,
        IDialogService dialogService)
    {
        this.logger = logger;
        this.dialogService = dialogService;
        logger.Trace("RecordLiveDataViewModel created");
        outputFileName = TempFileGenerator.CreateTempBinaryFileName();
    }

    #endregion

    #region UI Bound Properties

    public string ScanSystemFileName
    {
        get => scanSystemFileName;
        set
        {
            if (value == scanSystemFileName) return;
            scanSystemFileName = value;
            NotifyOfPropertyChange(() => ScanSystemFileName);
            ParseScanSystem();
        }
    }

   
    public string OutputFileName
    {
        get => outputFileName;
        set
        {
            if (value == outputFileName) return;
            outputFileName = value;
            NotifyOfPropertyChange(() => OutputFileName);
            NotifyOfPropertyChange(()=>CanStartRecording);
        }
    }

    public uint MinScanPeriod
    {
        get => minScanPeriod;
        set
        {
            if (value == minScanPeriod) return;
            minScanPeriod = value;
            NotifyOfPropertyChange(() => MinScanPeriod);
            NotifyOfPropertyChange(()=>CanStartRecording);
        }
    }

    public bool IsRecording
    {
        get => isRecording;
        set
        {
            if (value == isRecording) return;
            isRecording = value;
            NotifyOfPropertyChange(() => IsRecording);
            Refresh();
        }
    }

    public bool ParseOk => scanSystem != null;

    public string Message
    {
        get => message;
        set
        {
            if (value == message) return;
            message = value;
            NotifyOfPropertyChange(() => Message);
        }
    }

    #endregion
    
    #region Public Methods
    
    public void StartRecording()
    {
        IsRecording = true;
    }
    
    public void StopRecording()
    {
        IsRecording = false;
    }
    
    public void Close()
    {
        TryCloseAsync();
    }

    public void BrowseScanSystem()
    {
        var openFileDialogSettings = new OpenFileDialogSettings()
        {
            Title = "Open ScanSystem definition file",
            // InitialDirectory = initialDirectory,
            Filter = $"ScanSystem (*.json)|*.json|All Files (*.*)|*.*",
            CheckFileExists = true
        };

        bool? success = dialogService.ShowOpenFileDialog(this, openFileDialogSettings);
        if (success == true)
        {
            ScanSystemFileName = openFileDialogSettings.FileName;
        }
    }
    
    public void BrowseOutput()
    {
        var saveFileDialogSettings = new SaveFileDialogSettings()
        {
            Title = "Save raw data file",
            // InitialDirectory = initialDirectory,
            Filter = $"Raw Recording (*.bin)|*.bin|All Files (*.*)|*.*",
            CheckFileExists = false
        };

        bool? success = dialogService.ShowSaveFileDialog(this, saveFileDialogSettings);
        if (success == true)
        {
            OutputFileName = saveFileDialogSettings.FileName;
        }
    }
    #endregion

    #region Private Methods

    private void ParseScanSystem()
    {
        // Parse the scan system file
        
        try
        {
            scanSystem = ScanSystemParser.CreateFromFile(ScanSystemFileName, new ScanSystemCreationOptions(){});
            Message = "Parse OK";
        }
        catch (Exception e)
        {
            scanSystem = null;
            Message = "Parse Failed";
            logger.Error(e, "Failed to parse ScanSystem file");
        }
        Refresh();
    }


    #endregion
    
    #region Guard Methods
    
    public bool CanStartRecording => !IsRecording && ParseOk && !string.IsNullOrEmpty(OutputFileName)
                                     && MinScanPeriod is > 100 and < 10000;
    public bool CanStopRecording => IsRecording;
    public bool CanClose => !IsRecording;
    
    #endregion
}