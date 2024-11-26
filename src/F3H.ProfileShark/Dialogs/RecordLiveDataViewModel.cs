using ByteSizeLib;
using Caliburn.Micro;
using F3H.ProfileShark.Models;
using JoeScan.Pinchot;
using JoeScan.Pinchot.Parser;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;
using MvvmDialogs.FrameworkDialogs.SaveFile;
using NLog;
// ReSharper disable MemberCanBePrivate.Global

namespace F3H.ProfileShark.Dialogs;

// ReSharper disable once ClassNeverInstantiated.Global
public class RecordLiveDataViewModel : Screen
{
    private readonly ILogger logger;
    private readonly IDialogService dialogService;

    #region Private Fields

    private bool isRecording;
    private uint minScanPeriod = 5000;
    private string outputFileName;
    private string scanSystemFileName = string.Empty;
    ScanSystem? scanSystem;
    private LiveRecorder? recorder;
    private long bytesWritten;

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
        private set
        {
            if (value == outputFileName) return;
            outputFileName = value;
            NotifyOfPropertyChange(() => OutputFileName);
            NotifyOfPropertyChange(() => CanStartRecording);
            logger.Info($"OutputFileName set to {outputFileName}");
        }
    }

    public uint MinScanPeriod
    {
        get => minScanPeriod;
        set
        {
            if (value == minScanPeriod)
            {
                return;
            }

            minScanPeriod = value;
            logger.Info($"MinScanPeriod set to {minScanPeriod}");
            NotifyOfPropertyChange(() => MinScanPeriod);
            NotifyOfPropertyChange(() => CanStartRecording);
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
    public string BytesWritten { get; private set; } = String.Empty;
    public string ProfilesWritten { get; private set; } = String.Empty;

    #endregion

    #region Public Methods

    public async Task StartRecording()
    {
        
        bytesWritten = 0;
        recorder = IoC.Get<LiveRecorder>();
        recorder.OutputFileName = OutputFileName;
        recorder.ScanSystem = scanSystem;
        recorder.MinScanPeriod = MinScanPeriod;
        recorder.ProgressUpdate += HandleProgressUpdate;
        IsRecording = true;
        await recorder.StartRecording();
    }


    public void StopRecording()
    {
        if (recorder != null)
        {
            recorder.StopRecording();
            recorder.ProgressUpdate -= HandleProgressUpdate;
            scanSystem?.Dispose();
            scanSystem = null;
            recorder = null;
        }

        IsRecording = false;
    }

    public void Close()
    {
        TryCloseAsync(false);
    }

    public void CloseAndImport()
    {
        TryCloseAsync(true);
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

    private void HandleProgressUpdate(object? _, ProgressEventArgs progressEventArgs)
    {
        bytesWritten = progressEventArgs.BytesWritten;
        BytesWritten = $"Data: {ByteSize.FromBytes(progressEventArgs.BytesWritten).ToString("#.00")}";
        ProfilesWritten = $"Profiles: {progressEventArgs.ProfilesWritten}";
        NotifyOfPropertyChange(() => BytesWritten);
        NotifyOfPropertyChange(() => ProfilesWritten);
    }

    private void ParseScanSystem()
    {
        // Parse the scan system file

        try
        {
            scanSystem = ScanSystemParser.CreateFromFile(ScanSystemFileName);
            logger.Info($"Parsed ScanSystem file {ScanSystemFileName} successfully");
        }
        catch (Exception e)
        {
            scanSystem = null;
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
    public bool CanCloseAndImport => !IsRecording && !string.IsNullOrEmpty(OutputFileName) && bytesWritten > 0;

    #endregion
}