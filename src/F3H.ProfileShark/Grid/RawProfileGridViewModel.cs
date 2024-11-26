using System.Windows.Controls;
using Caliburn.Micro;
using F3H.ProfileShark.Models;
using NLog;

namespace F3H.ProfileShark.Grid;

public class RawProfileGridViewModel : Screen
{
    public DataManager Data { get; }
    public ILogger Logger { get; }



    public RawProfileGridViewModel(DataManager data, ILogger logger)
    {
        Data = data;
        Logger = logger;
    }

    protected override void OnViewAttached(object view, object context)
    {
        if (view is RawProfileGridView gv)
        {
            gv.Grid.SelectionChanged += GridOnSelectionChanged;
        }
    }

    private void GridOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var dg = sender as DataGrid;
        var rawProfile = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;

        if (dg != null && rawProfile != null)
        {
            dg.ScrollIntoView(rawProfile);
            dg.UpdateLayout();
        }
    }
}
