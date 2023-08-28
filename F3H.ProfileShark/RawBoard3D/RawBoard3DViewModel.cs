using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using F3H.ProfileShark.Helpers;
using F3H.ProfileShark.Models;
using HelixToolkit.Wpf;

namespace F3H.ProfileShark.RawBoard3D;

public class RawBoard3DViewModel : Screen
{
    public DataManager DataManager { get; }
    private readonly ItemColorService colorService;

    public bool ShowRaw
    {
        get => showRaw;
        set
        {
            if (value == showRaw) return;
            showRaw = value;
            NotifyOfPropertyChange(() => ShowRaw);
            UpdateBoardDisplay();
        }
    }

    public bool ShowModel
    {
        get => showModel;
        set
        {
            if (value == showModel) return;
            showModel = value;
            NotifyOfPropertyChange(() => ShowModel);
            UpdateBoardDisplay();
        }
    }

    public bool ShowColorCodedModel
    {
        get => showColorCodedModel;
        set
        {
            if (value == showColorCodedModel) return;
            showColorCodedModel = value;
            NotifyOfPropertyChange(() => ShowColorCodedModel);
            UpdateBoardDisplay();
        }
    }

    public RawBoard3DViewModel(DataManager dataManager,
        ItemColorService colorService)
    {
        DataManager = dataManager;
        this.colorService = colorService;
        DataManager.ProfileDataAdded += (_, _) => UpdateBoardDisplay();

        encoderPulseInterval = 0.0007914088801269108 * 4;
    }

    // private Visual3D ModelToVisual3D(StickModel model, bool showLabels = false)
    // {
    //     // var ptsDict = new Dictionary<byte, IList<Point3D>>();
    //     var group = new ModelVisual3D();
    //     bool alternate = false;
    //     foreach (var section in model.Sections)
    //     {
    //         alternate = !alternate;
    //         var visual = new PointsVisual3D
    //         {
    //             Color = alternate ? Colors.Red : Colors.Orange,
    //             Size = 1,
    //             Points = new Point3DCollection(section.TopPoints.Select(q => new Point3D(q.X, q.Y, q.Z)))
    //         };
    //         group.Children.Add(visual);
    //         if (showLabels)
    //         {
    //             group.Children.Add(new BillboardTextVisual3D()
    //             {
    //                 Background = new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
    //                 Position = new Point3D(section.CenterPoint.X, section.CenterPoint.Y + 20, section.CenterPoint.Z),
    //                 Text = $"D: {section.CenterPoint.Y:F1}mm"
    //             });
    //         }
    //     }
    //
    //     return group;
    // }

    // private Visual3D ModelToColorCodedVisual3D(StickModel model, bool showLabels = false)
    // {
    //     var ptsDict = new Dictionary<byte, IList<Point3D>>();
    //     foreach (var section in model.Sections)
    //     {
    //         foreach (var p in section.TopPoints)
    //         {
    //             var b = (byte)int.Clamp((int)((p.B + 1.0) / 2.0 * 255.0), 0, 255);
    //             if (!ptsDict.ContainsKey(b))
    //             {
    //                 ptsDict[b] = new List<Point3D>();
    //             }
    //
    //             ptsDict[b].Add(new Point3D(p.X, p.Y, p.Z));
    //         }
    //     }
    //
    //     var group = new ModelVisual3D();
    //
    //     foreach (byte col in ptsDict.Keys)
    //     {
    //         var visual = new PointsVisual3D
    //         {
    //             Color = colorService.DistancePalette[col],
    //             Size = 2,
    //             Points = new Point3DCollection(ptsDict[col])
    //         };
    //         group.Children.Add(visual);
    //     }
    //
    //     return group;
    // }

    private ModelVisual3D BoardToVisual3D(IEnumerable<RawProfile> board)
    {
        var first = board.First();
        var ptsDict = new Dictionary<byte, IList<Point3D>>();
        foreach (var profile in board)
        {
            var z = (profile.EncoderValue - first.EncoderValue) * encoderPulseInterval;

            foreach (var point2D in profile.Data)
            {
                var pt3d = new Point3D(point2D.X, point2D.Y, z);
                var colorValue = BinByBrightness(point2D.Brightness);
                if (!ptsDict.ContainsKey(colorValue))
                {
                    ptsDict[colorValue] = new List<Point3D>();
                }

                ptsDict[colorValue].Add(pt3d);
            }
        }

        var group = new ModelVisual3D();

        foreach (byte col in ptsDict.Keys)
        {
            var visual = new PointsVisual3D
            {
                Color = colorService.LogColorValues[col],
                Size = 2,
                Points = new Point3DCollection(ptsDict[col])
            };
            group.Children.Add(visual);
        }

        return group;
    }

    private static byte BinByBrightness(double b)
    {
        return (byte)(b); // clamp?
    }

    private void UpdateBoardDisplay()
    {
        PerspectiveVisual.Children.Clear();
        if (DataManager.Profiles.Count == 0)
        {
            return;
        }

        PerspectiveVisual.Children.Add(BoardToVisual3D(DataManager.Profiles));
    }


    #region Backing Properties

    private Point3D cursorPosition;
    private readonly double encoderPulseInterval;
    private Model3DGroup test;
    private bool showRaw = true;
    private bool showModel;
    private bool showColorCodedModel = false;

    #endregion

    #region Private Fields

    private HelixViewport3D? PerspectiveVP { get; set; }
    public ModelVisual3D PerspectiveVisual { get; private set; }

    #endregion

    #region UI Bound Properties

    public Point3D CursorPosition
    {
        get => cursorPosition;
        set
        {
            if (value.Equals(cursorPosition)) return;
            cursorPosition = value;
            NotifyOfPropertyChange(() => CursorPosition);
        }
    }

    #endregion

    #region IViewAware Implementation

    protected override void OnViewAttached(object view, object context)
    {
        if (view is RawBoard3DView lv)
        {
            PerspectiveVP = lv.PerspectiveVP;
            PerspectiveVisual = new ModelVisual3D();
            PerspectiveVP.Children.Add(PerspectiveVisual);
        }
    }

    #endregion
}