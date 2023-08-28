using System.ComponentModel;
using Caliburn.Micro;
using F3H.ProfileShark.Models;

namespace F3H.ProfileShark.ProfileDetail;

public class ProfileDetailViewModel : Screen
{
    private RawProfile? selectedProfile;
    public DataManager DataManager { get; }

   

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

    public ProfileDetailViewModel(DataManager dataManager)
    {
        DataManager = dataManager;
        dataManager.PropertyChanged += SelectedProfileChanged;
    }

    private void SelectedProfileChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(DataManager.SelectedProfile))
        {
            return;
        }

        SelectedProfile = DataManager.SelectedProfile;
    }
}
