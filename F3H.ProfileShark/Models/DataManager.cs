using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using Caliburn.Micro;
using F3H.ProfileShark.Helpers;
using NLog;

namespace F3H.ProfileShark.Models;

public class DataManager : PropertyChangedBase
{
    #region Events

    public event EventHandler ProfileDataAdded;
    public event EventHandler HeadSelectionChanged;
    public event EventHandler CameraSelectionChanged;

    #endregion

    #region Private Fields

    private RawProfile? selectedProfile;
    private readonly object locker = new();
    private readonly ICollectionView filteredView;

    private int scanHeadFilterById = -1;
    private int scanHeadFilterByCamera = 0;
    private double encoderPulseInterval;
    private bool useFlightsAndWindowFilter;

    #endregion

    #region Injected Properties

    public ILogger Logger { get; }

    #endregion

    #region UI Bound Properties

    public IObservableCollection<RawProfile> Profiles { get; } = new BindableCollection<RawProfile>();
    private List<RawProfile> originalData;
    private string currentFile = "";

    public ObservableCollection<KeyValuePair<int, string>> SelectableHeads { get; } =
        new ObservableCollection<KeyValuePair<int, string>>();
    public ObservableCollection<KeyValuePair<int, string>> SelectableCameras { get; } =
        new ObservableCollection<KeyValuePair<int, string>>()
        {
            new KeyValuePair<int, string>(0, "A/B"),
            new KeyValuePair<int, string>(1, "A"),
            new KeyValuePair<int, string>(2, "B"),
        };

    public int ScanHeadFilterById
    {
        get => scanHeadFilterById;
        set
        {
            if (value == scanHeadFilterById)
            {
                return;
            }
            scanHeadFilterById = value;
            NotifyOfPropertyChange(() => ScanHeadFilterById);
            filteredView.Refresh();
            OnHeadSelectionChanged();
        }
    }
    public int ScanHeadFilterByCamera
    {
        get => scanHeadFilterByCamera;
        set
        {
            if (value == scanHeadFilterByCamera)
            {
                return;
            }
            scanHeadFilterByCamera = value;
            NotifyOfPropertyChange(() => ScanHeadFilterByCamera);
            filteredView.Refresh();
            OnCameraSelectionChanged();
        }
    }
    public RawProfile? SelectedProfile
    {
        get => selectedProfile;
        set
        {
            if (Equals(value, selectedProfile))
            {
                return;
            }
            selectedProfile = value;
            NotifyOfPropertyChange(() => SelectedProfile);
        }
    }

    public double EncoderPulseInterval
    {
        get => encoderPulseInterval;
        set
        {
            if (value.Equals(encoderPulseInterval))
            {
                return;
            }
            encoderPulseInterval = value;
            NotifyOfPropertyChange(() => EncoderPulseInterval);
        }
    }

    public bool UseFlightsAndWindowFilter
    {
        get => useFlightsAndWindowFilter;
        set
        {
            if (value == useFlightsAndWindowFilter)
            {
                return;
            }
            useFlightsAndWindowFilter = value;
            FilterAndAdd();
            NotifyOfPropertyChange(() => UseFlightsAndWindowFilter);
        }
    }

    public string CurrentFile
    {
        get => currentFile;
        set
        {
            if (value == currentFile) return;
            currentFile = value;
            NotifyOfPropertyChange(() => CurrentFile);
            NotifyOfPropertyChange(() => CurrentFileShort);
        }
    }
    public string CurrentFileShort => String.IsNullOrEmpty(CurrentFile)? "No File Loaded" : Path.GetFileName(CurrentFile);

    #endregion


    #region Commands

    public RelayCommand GoToFirstProfileCommand { get; }
    public RelayCommand GoToLastProfileCommand { get; }
    public RelayCommand GoToNextProfileCommand { get; }
    public RelayCommand GoToPreviousProfileCommand { get; }

    #endregion

    #region Lifecycle

    public DataManager(ILogger logger)
    {
        Logger = logger;
        BindingOperations.EnableCollectionSynchronization(SelectableHeads, locker);
        filteredView = CollectionViewSource.GetDefaultView(Profiles);
        filteredView.Filter = Filter;
        GoToFirstProfileCommand = new RelayCommand((o) => GoToFirstProfile(),
            (o) => Profiles.Count > 0 && SelectedProfile != Profiles[0]);
        GoToLastProfileCommand = new RelayCommand((o) => GoToLastProfile(),
            (o) => Profiles.Count > 0 && SelectedProfile != Profiles[^1]);
        GoToNextProfileCommand = new RelayCommand((o) => GoToNextProfile(),
            (o) => Profiles.Count > 0 && SelectedProfile != Profiles[^1]);
        GoToPreviousProfileCommand = new RelayCommand((o) => GoToPreviousProfile(),
            (o) => Profiles.Count > 0 && SelectedProfile != Profiles[0]);
        EncoderPulseInterval = 1.0;
    }

    #endregion


    #region Public Methods

    public void SetProfiles(List<RawProfile> newProfiles)
    {  
        originalData = newProfiles;
        FilterAndAdd();
    }

    #endregion

    #region Private Methods

    private bool Filter(object obj)
    {
        if (ScanHeadFilterById == -1 && ScanHeadFilterByCamera == 0)
        {
            return true;
        }
        if (obj is RawProfile profile)
        {

            if (ScanHeadFilterById != -1 && profile.ScanHeadId != (uint)ScanHeadFilterById)
            {
                return false;
            }

            if (ScanHeadFilterByCamera != 0 && (int) profile.Camera != ScanHeadFilterByCamera)
            {
                return false;
            }

            return true;
        }

        return false;
    }

    private void FilterAndAdd()
    {
        Profiles.Clear();
        SelectableHeads.Clear();
        SelectableHeads.Add(new KeyValuePair<int, string>(-1, "*"));
        HashSet<uint> headsInFile = new HashSet<uint>();
        int index = 0;
        foreach (var p in originalData)
        {
            var rp = new RawProfile(p);
           
            Profiles.Add(rp);
            rp.ReducedTimeStampNs = rp.TimeStampNs - Profiles[0].TimeStampNs;
            rp.ReducedEncoder = rp.EncoderValue - Profiles[0].EncoderValue;
            headsInFile.Add(p.ScanHeadId);
        }
        foreach (uint headIds in headsInFile)
        {
            SelectableHeads.Add(new KeyValuePair<int, string>((int)headIds, $"{headIds}"));
        }
        OnProfileDataAdded();
    }

    #endregion


    #region Event Invocation

    protected virtual void OnProfileDataAdded()
    {
        ProfileDataAdded?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnHeadSelectionChanged()
    {
        HeadSelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnCameraSelectionChanged()
    {
        CameraSelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion

    #region UI Callbacks

   

    public void GoToFirstProfile()
    {
        if (scanHeadFilterById < 0)
        {
            SelectedProfile = Profiles[0];
        }
        else
        {
            SelectedProfile = Profiles.First(q => q.ScanHeadId == scanHeadFilterById);
        }
    }

    public void GoToLastProfile()
    {
        if (scanHeadFilterById < 0)
        {
            SelectedProfile = Profiles[^1];
        }
        else
        {
            SelectedProfile = Profiles.Last(q => q.ScanHeadId == scanHeadFilterById);
        }
    }

    public void GoToNextProfile()
    {

        if (scanHeadFilterById < 0)
        {
            // showing all heads, so just increase index
            SelectedProfile = Profiles[Profiles.IndexOf(SelectedProfile) + 1];
        }
        else
        {
            var idx = Profiles.IndexOf(SelectedProfile);
            var offset = 1;
            while (idx + offset < Profiles.Count && Profiles[idx + offset].ScanHeadId != scanHeadFilterById)
            {
                offset++;
            }
            SelectedProfile = Profiles[idx + offset];
        }

    }

    public void GoToPreviousProfile()
    {
        if (scanHeadFilterById < 0)
        {
            // showing all heads, so just decrease index
            SelectedProfile = Profiles[Profiles.IndexOf(SelectedProfile) - 1];
        }
        else
        {
            var idx = Profiles.IndexOf(SelectedProfile);
            var offset = -1;
            while (idx + offset >= 0 && Profiles[idx + offset].ScanHeadId != scanHeadFilterById)
            {
                offset--;
            }
            SelectedProfile = Profiles[idx + offset];
        }
    }

    #endregion
}
